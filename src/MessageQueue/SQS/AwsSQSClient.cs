using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.MessageQueue;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Microsoft.Extensions.Logging;
using MessageAttributeValue = Amazon.SQS.Model.MessageAttributeValue;

namespace Nwpie.Foundation.MessageQueue.SQS
{
    public class AwsSQSClient : MessageQueueBase, IAwsSQSClient
    {
        public AwsSQSClient(IConfigOptions<AwsSQS_Option> option, ISerializer serializer)
        {
            m_Serializer = serializer
                ?? throw new ArgumentNullException(nameof(ISerializer));
            m_Option = option
                ?? throw new ArgumentNullException(nameof(IConfigOptions<AwsSQS_Option>));

            if (ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), m_Option.Value?.ServiceUrl, m_Option.Value?.QueueBaseUrl, m_Option.Value?.Topic))
            {
                throw new ArgumentNullException($@"Options
                    {nameof(AwsSQS_Option.ServiceUrl)},
                    {nameof(AwsSQS_Option.QueueBaseUrl)},
                    {nameof(AwsSQS_Option.Topic)} are required. ");
            }

            DefaultTopic = m_Option.Value.Topic;
            var awsConfig = new AmazonSQSConfig
            {
                ServiceURL = m_Option.Value.ServiceUrl
            };

            if (m_Option.Value.AccessKey.HasValue() &&
                m_Option.Value.SecretKey.HasValue())
            {
                m_SQSClient = new AmazonSQSClient(m_Option.Value.AccessKey, m_Option.Value.SecretKey, awsConfig);
            }
            else
            {
                // Use Environment Variable
                m_SQSClient = new AmazonSQSClient(awsConfig);
            }
        }

        public async Task<IServiceResponse> PublishAsync<T>(T request) =>
            await PublishAsync<T>(DefaultTopic, request);

        public override async Task<IServiceResponse> PublishAsync<T>(string topic, T request)
        {
            var result = new ServiceResponse() as IServiceResponse;

            try
            {
                var requestToJsonString = typeof(T) == typeof(string)
                    ? request.ToString()
                    : m_Serializer.Serialize(request);
                var queueUrl = $"{m_Option.Value.QueueBaseUrl.TrimEndSlash()}/{m_Option.Value.Topic}";
                var sqsRequest = new SendMessageRequest()
                {
                    QueueUrl = queueUrl,
                    MessageBody = requestToJsonString,
                    MessageAttributes = new Dictionary<string, MessageAttributeValue>(StringComparer.OrdinalIgnoreCase)
                    {
                        { CommonConst.ApiName, new MessageAttributeValue(){ DataType = "String", StringValue = ServiceContext.ApiName }},
                        { CommonConst.ApiKey, new MessageAttributeValue(){ DataType = "String", StringValue = ServiceContext.ApiKey }},
                    }
                };

                if (m_Option.Value.Topic.EndsWith(MessageQueueConst.FIFO_Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    // ContentBase FIFO
                    sqsRequest.MessageGroupId = ServiceContext.ApiName;
                    sqsRequest.MessageDeduplicationId = CryptoUtils.GetSha256String(requestToJsonString);
                }

                var queued = await m_SQSClient.SendMessageAsync(sqsRequest);
                if (null != queued && queued.MessageId.HasValue())
                {
                    result.MsgId = queued.MessageId;
                    result.Success(StatusCodeEnum.Success, $"Finished to send topic={topic}, sqs={GetFullUrl(topic)}, messageId={queued.MessageId}, request={requestToJsonString}. ");
                }
                else
                {
                    Logger.LogError($"Failed to send topic={topic}, sqs={GetFullUrl(topic)}, request={requestToJsonString}. ");
                    result.Error(StatusCodeEnum.Error, $"Failed to send topic={topic}, sqs={GetFullUrl(topic)}, request={requestToJsonString}. ");
                }
            }
            catch (AmazonSQSException ex)
            {
                Logger.LogError($"AmazonSQSException, topic={topic}, sqs={GetFullUrl(topic)}, request={request?.ToString()}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, sqs={GetFullUrl(topic)}, request={request?.ToString()}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public async Task<IServiceResponse<List<T>>> ConsumeAsync<T>(bool autoAck = true, ushort? maxMessages = null) =>
            await ConsumeAsync<T>(DefaultTopic, autoAck, maxMessages);

        public override async Task<IServiceResponse<List<T>>> ConsumeAsync<T>(string topic, bool autoAck = true, ushort? maxMessages = null)
        {
            var result = new ServiceResponse<List<T>>
            {
                Data = new List<T>()
            };

            try
            {
                var queueUrl = $"{m_Option.Value.QueueBaseUrl.TrimEndSlash()}/{topic}";
                var message = await m_SQSClient.ReceiveMessageAsync(new ReceiveMessageRequest()
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = m_Option.Value.MaxConsumeCount ?? maxMessages ?? 1
                });

                foreach (var messageDto in message.Messages)
                {
                    T entry;
                    if (typeof(T) == typeof(string))
                    {
                        entry = (T)Convert.ChangeType(messageDto.Body, typeof(string));
                    }
                    else
                    {
                        entry = m_Serializer.Deserialize<T>(messageDto.Body, ignoreException: true);
                    }

                    var command = new CommandModel<T>
                    {
                        Topic = topic,
                        Raw = messageDto.Body,
                    }.Success().Content(entry) as ICommandModel;

                    _ = Task.Run(() => OnConsumed(topic, command)).ConfigureAwait(false);

                    if (null != entry)
                    {
                        result.Data.Add(entry);
                    }
                }

                if (message.Messages?.Count() > 0 && autoAck)
                {
                    var batch = message.Messages.ConvertAll(o => new DeleteMessageBatchRequestEntry()
                    {
                        Id = o.MessageId,
                        ReceiptHandle = o.ReceiptHandle
                    });

                    var deleteResult = await m_SQSClient.DeleteMessageBatchAsync(queueUrl, batch);
                    if (deleteResult.Failed?.Count() > 0)
                    {
                        throw new Exception($"Consume topic={topic} from SQS failed, count={message.Messages?.Count()}, fail_count={deleteResult.Failed?.Count()}. ");
                    }
                }

                result.Success(StatusCodeEnum.Success,
                    $"Finished to consume topic={topic}, messageId={result.MsgId}, count={message.Messages?.Count()}. ");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception, topic={topic}, queueUrl={GetFullUrl(topic)}, ex={ex} ");
                result.Error(StatusCodeEnum.Exception, ex);
            }

            return result;
        }

        public string GetFullUrl(string topic = null) =>
            $"{m_Option?.Value?.QueueBaseUrl.TrimEndSlash()}/{topic ?? m_Option?.Value?.Topic}";

        public override void Dispose()
        {
            m_SQSClient?.Dispose();
        }

        public string DefaultTopic { get; set; }

        protected readonly ISerializer m_Serializer;
        protected readonly AmazonSQSClient m_SQSClient;
        protected readonly IConfigOptions<AwsSQS_Option> m_Option;
    }
}
