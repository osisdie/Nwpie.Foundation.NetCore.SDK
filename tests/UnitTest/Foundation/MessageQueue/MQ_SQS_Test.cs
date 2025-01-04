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
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.MessageQueue
{
    [Collection("MessageQueue_SQS_Test")]
    public class MQ_SQS_Test : TestBase
    {
        public MQ_SQS_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "TODO")]
        //[Fact(Skip = "Make sure SQS is connective")]
        public async Task Integration_Test()
        {
            await Client_Publish_Test();
        }

        async Task SQS_Config_Test()
        {
            var multipleKeys = new List<ConfigItem>() {
                new ConfigItem()
                {
                    ConfigKey = SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey
                }
            };

            var response = await DefaultConfigServer.GetLatest(multipleKeys);
            Assert.True(response.IsSuccess);
            Assert.NotEmpty(response.Data);
            Assert.Contains(SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey, response.Data.Keys);
            Assert.False(string.IsNullOrWhiteSpace(
                response.Data[SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey])
            );

            var serializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            Assert.NotNull(serializer);

            var opt = serializer.Deserialize<AwsSQS_Option>(
                response.Data[SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey]
            );
            Assert.False(string.IsNullOrWhiteSpace(opt.ServiceUrl));
            Assert.False(string.IsNullOrWhiteSpace(opt.QueueBaseUrl));
            Assert.False(string.IsNullOrWhiteSpace(opt.Topic));

            m_Option = new ConfigOptions<AwsSQS_Option>(opt);
            var tryCreate1 = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>()?.GetMessageQueue(MessageQueueProviderEnum.SQS, m_Option);
            Assert.NotNull(tryCreate1);
            Assert.True(tryCreate1.IsSuccess);
            Assert.NotNull(tryCreate1.Data);

            var tryCreate2 = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>()?.GetMessageQueue(MessageQueueProviderEnum.SQS, m_Option);
            Assert.NotNull(tryCreate2);
            Assert.True(tryCreate2.IsSuccess);
            Assert.NotNull(tryCreate2.Data);
            Assert.Same(tryCreate1.Data, tryCreate2.Data);

        }

        private async Task Client_Publish_Test()
        {
            {
                var sqsClient = ComponentMgr.Instance.TryResolve<IAwsSQSClient>();
                Assert.NotNull(sqsClient);

                var simpleCall = await sqsClient.PublishAsync(new NotifySend_RequestModel()
                {
                    Kind = (byte)NotifyChannelEnum.Email,
                    Title = $"{ServiceContext.ApiName}-{MethodBase.GetCurrentMethod()}-UnitTest",
                    Message = "Awesome !",
                    ToList = "dev@kevinw.net",
                    Sender = sqsClient.GetType().ToString()
                }.PreProcess());
                Assert.True(simpleCall.IsSuccess);
            }

            {
                var notifyClient = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>();
                Assert.NotNull(notifyClient);

                var optFromConfig = new ConfigOptions<AwsSQS_Option>(
                    SysConfigKey
                        .Default_AWS_SQS_Urls_Notification_ConfigKey
                        .ConfigServerValue<AwsSQS_Option>()
                );
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.ServiceUrl));
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.QueueBaseUrl));
                Assert.False(string.IsNullOrWhiteSpace(optFromConfig.Value.Topic));

                var mqClient = notifyClient.GetMessageQueue(MessageQueueProviderEnum.SQS, optFromConfig)
                    .Data as IAwsSQSClient;
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

        [Fact(Skip = "TODO")]
        public async Task Client_Consume_Test()
        {
            var simpleCall = await m_SQS.ConsumeAsync<NotifySend_RequestModel>(
                autoAck: false,
                maxMessages: 1
            );
            Assert.True(simpleCall.IsSuccess);
        }

        public override async Task<bool> IsReady()
        {
            await SQS_Config_Test();

            m_SQS = ComponentMgr.Instance.TryResolve<IAwsSQSClient>();
            Assert.NotNull(m_SQS);

            m_SQS.ConsumedEvent += OnConsumed;

            return true;
        }

        private void OnConsumed(string topic, ICommandModel message)
        {
            Console.WriteLine($"received from topic(={topic}), msg={message?.Raw}");
        }

        protected ConfigOptions<AwsSQS_Option> m_Option;
        protected IAwsSQSClient m_SQS;
    }
}
