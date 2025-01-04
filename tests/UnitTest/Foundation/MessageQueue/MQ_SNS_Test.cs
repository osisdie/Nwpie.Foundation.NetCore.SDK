using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Notification.Extensions;
using Nwpie.Foundation.Location.Contract;
using Nwpie.Foundation.MessageQueue.SNS;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.MessageQueue
{
    [Collection("MessageQueue_SNS_Test")]
    public class MQ_SNS_Test : TestBase
    {
        public MQ_SNS_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test aws service")]
        public async Task Integration_Test()
        {
            await Create_SQS_Test();
            await Create_Subscription_Test();
            //await Client_Publish_Test();
        }

        async Task SQS_Config_Test()
        {
            var multipleKeys = new List<ConfigItem>() {
                new ConfigItem()
                {
                    ConfigKey = SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey
                },
                new ConfigItem()
                {
                    ConfigKey = SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey
                }
            };

            var response = await DefaultConfigServer.GetLatest(multipleKeys);
            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Data);
            Assert.False(string.IsNullOrWhiteSpace(response.Data[SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey]));
            Assert.False(string.IsNullOrWhiteSpace(response.Data[SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey]));

            {
                var opt = Serializer.Deserialize<AwsSQS_Option>(
                    response.Data[SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey]
                );
                Assert.False(string.IsNullOrWhiteSpace(opt.ServiceUrl));
                Assert.False(string.IsNullOrWhiteSpace(opt.QueueBaseUrl));
                Assert.False(string.IsNullOrWhiteSpace(opt.HomeArn));
                Assert.False(string.IsNullOrWhiteSpace(opt.Topic));

                m_SqsOption = new ConfigOptions<AwsSQS_Option>(opt);
            }

            {
                var opt = Serializer.Deserialize<AwsSNS_Option>(
                    response.Data[SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey]
                );
                Assert.False(string.IsNullOrWhiteSpace(opt.ServiceUrl));
                Assert.False(string.IsNullOrWhiteSpace(opt.HomeArn));
                Assert.False(string.IsNullOrWhiteSpace(opt.Topic));

                m_SnsOption = new ConfigOptions<AwsSNS_Option>(opt);
            }

            var tryCreate1 = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>()?.GetMessageQueue(MessageQueueProviderEnum.SQS, m_SqsOption);
            Assert.NotNull(tryCreate1);
            Assert.True(tryCreate1.IsSuccess);
            Assert.NotNull(tryCreate1.Data);

            var tryCreate2 = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>()?.GetMessageQueue(MessageQueueProviderEnum.SQS, m_SqsOption);
            Assert.NotNull(tryCreate2);
            Assert.True(tryCreate2.IsSuccess);
            Assert.NotNull(tryCreate2.Data);
            Assert.Same(tryCreate1.Data, tryCreate2.Data);
        }

        async Task Create_SQS_Test()
        {
            //await DeleteSQS_Test();
            // Create new
            var createQueueResponse = await m_SNSClient.GetQueueUrlOrAddNewQueue(m_QueueName);
            Assert.NotNull(createQueueResponse?.Data);
            m_QueueUrl = createQueueResponse.Data;

            var lookupResponse = await m_SNSClient.LookupArn(m_QueueUrl);
            Assert.NotNull(lookupResponse?.Data);
            m_QueueArn = lookupResponse.Data;
        }

        async Task Create_Subscription_Test()
        {
            //await Unsubscribe_Test();
            // create new
            var subscribeResponse = await m_SNSClient.SubscribeAsync(m_QueueArn);
            Assert.NotNull(subscribeResponse?.Data);
            m_SubscriptionArn = subscribeResponse.Data;

            //await Unsubscribe_Test();
            //await DeleteSQS_Test();
        }

        private async Task DeleteSQS_Test()
        {
            if (string.IsNullOrWhiteSpace(m_QueueUrl))
            {
                return;
            }

            try
            {
                var delResponse = await m_SNSClient.TryDeleteQueue(m_QueueUrl);
            }
            catch { }
        }

        private async Task Client_PublishEmail_Test()
        {
            var simpleCall = await m_DefaultSQS.PublishAsync(new NotifySend_RequestModel()
            {
                Kind = (byte)NotifyChannelEnum.Email,
                Title = $"{ServiceContext.ApiName}-{MethodBase.GetCurrentMethod()}-UnitTest",
                Message = "Awesome !",
                ToList = "dev@kevinw.net",
                Sender = m_DefaultSQS.GetType().ToString()
            }.PreProcess());
            Assert.True(simpleCall.IsSuccess);

            {
                var optFromConfig = new ConfigOptions<AwsSQS_Option>(
                    SysConfigKey
                        .Default_AWS_SQS_Urls_Notification_ConfigKey
                        .ConfigServerValue<AwsSQS_Option>()
                );
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.ServiceUrl));
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.QueueBaseUrl));
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.Topic));

                var mqClient = m_SQSFactory.GetMessageQueue(MessageQueueProviderEnum.SQS, optFromConfig)
                    ?.Data as IAwsSQSClient;
                Assert.NotNull(mqClient);

                var complicatedCall = await mqClient.PublishAsync(new NotifySend_RequestModel()
                {
                    Kind = (byte)NotifyChannelEnum.Email,
                    Title = $"{ServiceContext.ApiName}-{MethodBase.GetCurrentMethod()}-UnitTest",
                    Message = "Awesome !",
                    ToList = "dev@kevinw.net",
                    Sender = mqClient.GetType().ToString()
                }.PreProcess());
                Assert.True(complicatedCall.IsSuccess);
            }
        }

        [Fact(Skip = "Won't test aws service")]
        public async Task Client_ConsumeEmail_Test()
        {
            var simpleCall = await m_DefaultSQS.ConsumeAsync<NotifySend_RequestModel>(
                autoAck: false,
                maxMessages: 1
            );
            Assert.True(simpleCall.IsSuccess);
        }

        [Fact(Skip = "Won't test aws service")]
        public async Task Client_BroadcaseLocation_Test()
        {
            var listResponse = await m_SNSClient.ListSubscriptions();
            Assert.NotEmpty(listResponse?.Data);

            foreach (var sub in listResponse.Data)
            {
                var response = await m_SNSClient.BroadcastAsync<string>(
                    $"[{ServiceContext.ApiName}][{MethodBase.GetCurrentMethod()}] UnitTest"
                );

                Assert.True(response?.IsSuccess);
                Assert.NotEmpty(response?.Data);
            };

            await Client_ConsumeLocation_Test();
        }

        [Fact(Skip = "Won't test aws service")]
        public async Task Client_ConsumeLocation_Test()
        {
            var opt = new ConfigOptions<AwsSQS_Option>(new AwsSQS_Option()
            {
                ServiceUrl = m_SqsOption.Value.ServiceUrl,
                QueueBaseUrl = m_SqsOption.Value.QueueBaseUrl,
                Topic = m_QueueName
            });

            m_SQS_ListeningSNS = m_SQSFactory
                .GetMessageQueue(MessageQueueProviderEnum.SQS, opt)
                .Data
                as IAwsSQSClient;
            Assert.NotNull(m_SQS_ListeningSNS);

            m_SQS_ListeningSNS.ConsumedEvent += OnConsumed;

            {
                var consumeResult = await m_SQS_ListeningSNS.ConsumeAsync<string>(m_QueueName, true, 1);
                Assert.True(consumeResult.IsSuccess);
                Assert.NotEmpty(consumeResult.Data);
            }
        }

        private void OnConsumed(string topic, ICommandModel message)
        {
            Console.WriteLine($"received from topic(={topic}), msg={message?.Raw}");
        }

        private void OnBroadcasted(string topic, string message, int numOfSubscriptions)
        {
            Console.WriteLine($"broadcast topic(={topic}), msg={message}");
        }

        public override async Task<bool> IsReady()
        {
            await SQS_Config_Test();

            m_SQSFactory = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>();
            Assert.NotNull(m_SQSFactory);

            m_DefaultSQS = ComponentMgr.Instance.TryResolve<IAwsSQSClient>();
            Assert.NotNull(m_DefaultSQS);

            m_QueueName = LocationServiceConfig.CreateQueueName(
                ServiceContext.ApiName,
                ServiceContext.MachineName
            );
            m_QueueArn = $"{m_SqsOption.Value.HomeArn}:{m_QueueName}";
            m_QueueUrl = $"{m_SqsOption.Value.QueueBaseUrl}/{m_QueueName}";

            m_SNSClient = new AwsNotificationClient(
                m_SnsOption,
                Serializer
            );

            m_SNSClient.BroadcastedEvent += OnBroadcasted;
            m_SnsArn = $"{m_SnsOption.Value.HomeArn}:{m_SnsOption.Value.Topic}";

            return true;
        }

        protected string m_QueueName;
        protected string m_QueueArn;
        protected string m_QueueUrl;
        protected string m_SnsArn;
        protected string m_SubscriptionArn;
        protected ConfigOptions<AwsSQS_Option> m_SqsOption;
        protected ConfigOptions<AwsSNS_Option> m_SnsOption;
        protected IMessageQueueFactory m_SQSFactory;
        protected IAwsSQSClient m_DefaultSQS;
        protected IAwsSQSClient m_SQS_ListeningSNS;
        protected IAwsNotificationClient m_SNSClient;
    }
}
