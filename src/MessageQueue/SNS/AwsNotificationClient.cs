using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Auth.AccessControlPolicy;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.MessageQueue.SNS
{
    public class AwsNotificationClient : CObject, IAwsNotificationClient
    {
        public AwsNotificationClient(IConfigOptions<AwsSNS_Option> option, ISerializer serializer)
        {
            m_Serializer = serializer ?? throw new ArgumentNullException(nameof(ISerializer));
            m_Option = option ?? throw new ArgumentNullException(nameof(IConfigOptions<AwsSNS_Option>));

            if (ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), m_Option.Value?.ServiceUrl, m_Option.Value?.HomeArn, m_Option.Value?.Topic))
            {
                throw new ArgumentNullException($@"Options
                    {nameof(AwsSNS_Option.ServiceUrl)},
                    {nameof(AwsSNS_Option.HomeArn)},
                    {nameof(AwsSNS_Option.Topic)} are required. ");
            }

            DefaultTopic = m_Option.Value.Topic;
            m_Arn = $"{m_Option.Value.HomeArn}:{DefaultTopic}";
            var awsConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = m_Option.Value.ServiceUrl
            };

            if (m_Option.Value.AccessKey.HasValue() &&
                m_Option.Value.SecretKey.HasValue())
            {
                m_SNSClient = new AmazonSimpleNotificationServiceClient(m_Option.Value.AccessKey, m_Option.Value.SecretKey, awsConfig);
                m_SQSClient = new AmazonSQSClient(m_Option.Value.AccessKey, m_Option.Value.SecretKey, RegionEndpoint.GetBySystemName(m_Option.Value.Region));
            }
            else
            {
                // Use Environment Variable
                m_SNSClient = new AmazonSimpleNotificationServiceClient(awsConfig);
                m_SQSClient = new AmazonSQSClient(RegionEndpoint.GetBySystemName(m_Option.Value.Region));
            }
        }

        public async Task<IServiceResponse<List<Subscription>>> ListSubscriptions(string topic = null)
        {
            var result = new ServiceResponse<List<Subscription>>();
            try
            {
                var response = await m_SNSClient.ListSubscriptionsByTopicAsync(
                    topicArn: GetFullArn(topic)
                );

                if (HttpStatusCode.OK == response?.HttpStatusCode ||
                   HttpStatusCode.NotFound == response?.HttpStatusCode)
                {
                    result.Success();
                }

                if (true == response?.Subscriptions.Count() > 0)
                {
                    result.Content(response.Subscriptions);
                }
                else
                {
                    result.Success(StatusCodeEnum.EmptyData);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, sns={GetFullArn(topic)}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse<List<string>>> BroadcastAsync<T>(T request) =>
            await BroadcastAsync(DefaultTopic, request);

        public async Task<IServiceResponse<List<string>>> BroadcastAsync<T>(
            string topic,
            T request)
        {
            var result = new ServiceResponse<List<string>>()
            {
                Data = new List<string>(),
                ErrMsg = ""
            };

            try
            {
                var message = typeof(T) == typeof(string)
                    ? request.ToString()
                    : m_Serializer.Serialize(request);
                string nextToken = null;
                do
                {
                    var topicResponse = await m_SNSClient.ListSubscriptionsByTopicAsync(new ListSubscriptionsByTopicRequest()
                    {
                        TopicArn = GetFullArn(topic),
                        NextToken = nextToken
                    });

                    if (true == topicResponse.Subscriptions.Count() > 0)
                    {
                        var publishResult = await SendAsync(message,
                            topicResponse.Subscriptions
                        );
                        result.Data.AddRange(publishResult?.Data ?? new List<string>());
                    }

                    nextToken = topicResponse?.NextToken;

                } while (nextToken.HasValue());

                if (result.Data.Count() > 0)
                {
                    result.Success();
                }
                else
                {
                    result.Success(StatusCodeEnum.EmptyData);
                }

                _ = Task.Run(() => OnBroadcasted(m_Arn, message, result.Data.Count())).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, sns={GetFullArn(topic)}, request={request?.ToString()}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse<string>> SubscribeAsync(string endpoint, string protocol = "sqs", Dictionary<string, string> attributes = null)
        {
            var result = new ServiceResponse<string>();
            if (true != endpoint.HasValue())
            {
                return result.Error(StatusCodeEnum.InvalidParameter, nameof(endpoint));
            }

            try
            {
                attributes = attributes ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                MergeAttribute(protocol, attributes);

                var subResponse = await m_SNSClient.SubscribeAsync(new SubscribeRequest()
                {
                    TopicArn = GetFullArn(),
                    Protocol = protocol,
                    Endpoint = endpoint,
                    Attributes = attributes,
                    ReturnSubscriptionArn = true
                });

                if (true == subResponse?.SubscriptionArn.HasValue())
                {
                    Logger.LogInformation($"Successfully created subscriptionArn={subResponse.SubscriptionArn}, sns={GetFullArn()}, endpoint={endpoint}, protocol={protocol}. ");
                    result.Success().Content(subResponse.SubscriptionArn);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception on subscribed endpoint(={endpoint}, protocol={protocol}) to sns={GetFullArn()}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse> UnsubscribeAsync(string subArn)
        {
            var result = new ServiceResponse();
            if (true != subArn.HasValue())
            {
                return result.Error(StatusCodeEnum.InvalidParameter, nameof(subArn));
            }

            try
            {
                var unsubResponse = await m_SNSClient.UnsubscribeAsync(subArn);
                if (HttpStatusCode.OK == unsubResponse?.HttpStatusCode ||
                    HttpStatusCode.NotFound == unsubResponse?.HttpStatusCode)
                {
                    result.Success();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception on unsubscribed subscriptionArn={subArn} from sns={GetFullArn()}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }


        public async Task<IServiceResponse<string>> GetQueueUrlOrAddNewQueue(string queueName)
        {
            var result = new ServiceResponse<string>();

            try
            {
                var getQueueUrl = await m_SQSClient.GetQueueUrlAsync(new GetQueueUrlRequest
                {
                    QueueName = queueName
                });

                if (true == getQueueUrl?.QueueUrl.HasValue())
                {
                    Logger.LogInformation($"Retrieve existing sqs={queueName}, url={getQueueUrl.QueueUrl}. ");
                    return result.Success().Content(getQueueUrl.QueueUrl);
                }
            }
            catch { }

            try
            {
                var createQueueResponse = await m_SQSClient.CreateQueueAsync(new CreateQueueRequest
                {
                    QueueName = queueName,
                });

                if (true == createQueueResponse?.QueueUrl.HasValue())
                {
                    _ = GrantPermission(createQueueResponse.QueueUrl);

                    Logger.LogInformation($"Created new sqs={queueName}, url={createQueueResponse.QueueUrl}. ");
                    return result.Success()
                        .Content(createQueueResponse.QueueUrl);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, sqs={queueName}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse> TryDeleteQueue(string queueUrl)
        {
            var result = new ServiceResponse();
            try
            {
                var response = await m_SQSClient.DeleteMessageAsync(new DeleteMessageRequest
                {
                    QueueUrl = queueUrl
                });

                if (HttpStatusCode.OK == response?.HttpStatusCode ||
                   HttpStatusCode.NotFound == response?.HttpStatusCode)
                {
                    Logger.LogInformation($"Successfully deleted sqs={queueUrl} ");
                    result.Success();
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse> GrantPermission(string queueUrl)
        {
            var result = new ServiceResponse();
            var arnResponse = await LookupArn(queueUrl);
            if (true != arnResponse?.Data.HasValue())
            {
                return result;
            }

            var policy = new Policy()
            {
                Statements = new List<Statement>() {
                    new Statement(Statement.StatementEffect.Allow) {
                        Principals = new List<Principal>() { new Principal("*") },
                        Actions = new List<ActionIdentifier>() {
                            new ActionIdentifier("SQS:ReceiveMessage"),
                            new ActionIdentifier("SQS:SendMessage")
                        },
                        Resources = new List<Resource>() { new Resource(arnResponse.Data) }
                    }
                },
            };

            var queueAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { QueueAttributeName.Policy.ToString(), policy.ToJson() }
            };

            try
            {
                var response = await m_SQSClient.SetQueueAttributesAsync(
                    new SetQueueAttributesRequest(queueUrl, queueAttributes)
                );

                if (HttpStatusCode.OK == response?.HttpStatusCode ||
                   HttpStatusCode.NotFound == response?.HttpStatusCode)
                {
                    result.Success();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, queueUrl={queueUrl}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse<string>> LookupArn(string queueUrl)
        {
            var result = new ServiceResponse<string>();
            if (true != queueUrl.HasValue())
            {
                return result.Error(StatusCodeEnum.InvalidParameter, nameof(queueUrl));
            }

            try
            {
                var getQueueAttributes = await m_SQSClient.GetQueueAttributesAsync(
                    queueUrl,
                    new List<string>() { "QueueArn" }
                );

                if (true == getQueueAttributes?.Attributes?.ContainsKey("QueueArn"))
                {
                    return result.Success()
                        .Content(getQueueAttributes.Attributes["QueueArn"]);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, queueUrl={queueUrl}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        async Task<IServiceResponse<List<string>>> SendAsync(string message, List<Subscription> subscription)
        {
            var result = new ServiceResponse<List<string>>
            {
                Data = new List<string>(),
                ErrMsg = ""
            };

            foreach (var sub in subscription)
            {
                try
                {
                    var response = await m_SNSClient.PublishAsync(
                        topicArn: m_Arn,
                        message: message
                    );

                    if (null != response && response.MessageId.HasValue())
                    {
                        result.Data.Add(sub.Endpoint);
                        Logger.LogInformation($"Finished to send from sns={GetFullArn()} to sqs={sub.SubscriptionArn}, messageId={response.MessageId}, msg={message}. ");
                    }
                    else
                    {
                        var errMsg = $"Failed to send from sns={GetFullArn()} to sqs={sub.SubscriptionArn}, msg={message}. ";
                        result.ErrMsg += errMsg;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to send from sns={GetFullArn()} to sqs={sub.SubscriptionArn}, msg={message}, ex={ex} ");
                }
            };

            if (result.Data?.Count() > 0)
            {
                result.Success();
            }
            return result;
        }

        void MergeAttribute(string protocol, Dictionary<string, string> attributes)
        {
            if (DefaultProtocol == protocol &&
               true != attributes?.ContainsKey(DefaultNameOfRawMessageDelivery) &&
               null != attributes)
            {
                attributes.Add(DefaultNameOfRawMessageDelivery, DefaultValueOfRawMessageDelivery);
            }
        }

        public string GetFullArn(string topic = null) =>
            $"{m_Option?.Value?.HomeArn}:{topic ?? m_Option?.Value?.Topic}";

        public virtual void OnBroadcasted(string topic, string message, int numOfSubscriptions) =>
            BroadcastedEvent?.Invoke(topic, message, numOfSubscriptions);

        public void Dispose()
        {
            m_SNSClient?.Dispose();
        }

        public string DefaultTopic { get; set; }
        public string DefaultProtocol { get; set; } = "sqs";
        public string DefaultNameOfRawMessageDelivery { get; set; } = "RawMessageDelivery";
        public string DefaultValueOfRawMessageDelivery { get; set; } = "true";
        public event Action<string, string, int> BroadcastedEvent;

        protected readonly string m_Arn;
        protected readonly ISerializer m_Serializer;
        protected readonly IConfigOptions<AwsSNS_Option> m_Option;
        protected readonly AmazonSQSClient m_SQSClient;
        protected readonly AmazonSimpleNotificationServiceClient m_SNSClient;
    }
}
