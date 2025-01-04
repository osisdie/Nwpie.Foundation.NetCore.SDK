using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Notification.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Notification
{
    public class Notification_Client_Test : TestBase
    {
        public Notification_Client_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote notification service")]
        public async Task Specific_Env_Notify_Test()
        {
            var env = EnvironmentEnum.Development;
            var exists = ConfigKeyForSetForAllEnvMap.TryGetValue(
                SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey, out var items
            );
            Assert.True(exists);

            var sqsOption = Serializer.Deserialize<AwsSQS_Option>(
                items[env.GetDisplayName()]
            );
            Assert.NotNull(sqsOption);
            Assert.NotNull(sqsOption.ServiceUrl);
            Assert.NotNull(sqsOption.QueueBaseUrl);
            Assert.NotNull(sqsOption.Topic);

            var configOptions = new ConfigOptions<AwsSQS_Option>(sqsOption);
            var tryCreateClient = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>()?.GetMessageQueue(MessageQueueProviderEnum.SQS, configOptions);
            Assert.NotNull(tryCreateClient);
            Assert.NotNull(tryCreateClient.Data);

            var notifyClient = tryCreateClient.Data;
            var dto = new NotifySend_RequestModel()
            {
                Kind = (byte)NotifyChannelEnum.Email,
                Title = $"todo.{env}-{ServiceContext.ApiName}-UnitTest",
                Message = "Awesome !",
                ToList = "dev@kevinw.net",
                Greeting = ServiceContext.ApiName,
                Sender = ServiceContext.ApiName
            }.PreProcess();

            var result = await notifyClient.PublishAsync(sqsOption.Topic, dto);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact(Skip = "Won't test remote notification service")]
        public async Task Email_Test()
        {
            foreach (var env in TodoApiKeyList.Take(1))
            {
                var dto = new NotifySend_RequestModel()
                {
                    Kind = (byte)NotifyChannelEnum.Email,
                    Title = $"todo.{env.Key}-{ServiceContext.ApiName}-UnitTest",
                    Message = "Awesome !",
                    ToList = "dev@kevinw.net",
                    Greeting = ServiceContext.ApiName,
                    Sender = ServiceContext.ApiName
                }.PreProcess();

                var request = new NotifySend_Request()
                {
                    Data = dto
                };

                var serialized = Serializer.Serialize(request);

                // SDK
                {
                    var response = await m_NotifySQSClient.SendAsync(request.Data);
                    Assert.NotNull(response);
                    Assert.True(response.IsSuccess);
                }

                // API
                //{
                //    var response = await
                //          m_notifyHttpClient.SendAsync(request);
                //    Assert.NotNull(response);
                //    Assert.True(response.IsSuccess);
                //}
            }
        }

        [Fact(Skip = "Won't test remote notification service")]
        public async Task Line_Test()
        {
            foreach (var env in TodoApiKeyList.Take(1))
            {
                var dto = new NotifySend_RequestModel()
                {
                    Kind = (byte)NotifyChannelEnum.Line,
                    Title = $"todo.{env.Key}-{ServiceContext.ApiName}-UnitTest",
                    Message = "Awesome !",
                    ToList = "fake_LINE_channel_id",
                    ApiName = ServiceContext.ApiName,
                    ApiKey = ServiceContext.ApiKey
                };

                var request = new NotifySend_Request()
                {
                    Data = dto
                };

                // SDK
                {
                    var response = await m_NotifySQSClient.SendAsync(request.Data);
                    Assert.NotNull(response);
                    Assert.True(response.IsSuccess);
                }

                // API
                //{
                //    var response = await
                //          m_notifyHttpClient.SendAsync(request);
                //    Assert.NotNull(response);
                //    Assert.True(response.IsSuccess);
                //}
            }
        }

        [Fact(Skip = "Won't test remote notification service")]
        public async Task Slack_Test()
        {
            foreach (var env in TodoApiKeyList.Take(1))
            {
                var dto = new NotifySend_RequestModel()
                {
                    Kind = (byte)NotifyChannelEnum.Slack,
                    Title = $"todo.{env.Key}-{ServiceContext.ApiName}-UnitTest",
                    Message = "Awesome !",
                    ToList = "#foundation-notify-dev",
                    ApiName = ServiceContext.ApiName,
                    ApiKey = ServiceContext.ApiKey
                };

                var request = new NotifySend_Request()
                {
                    Data = dto
                };

                // SDK
                {
                    var response = await m_NotifySQSClient.SendAsync(request.Data);
                    Assert.NotNull(response);
                    Assert.True(response.IsSuccess);
                }

                // API
                //{
                //    var response = await
                //      m_notifyHttpClient.SendAsync(request);
                //    Assert.NotNull(response);
                //    Assert.True(response.IsSuccess);
                //}
            }
        }

        public override async Task<bool> IsReady()
        {
            if (null != m_NotifyHttpClient && null != m_NotifySQSClient)
            {
                return true;
            }

            if (null == m_NotifyHttpClient)
            {
                m_NotifyHttpClient = ComponentMgr.Instance.TryResolve<INotificationHttpClient>();
                Assert.NotNull(m_NotifyHttpClient);

                var isReady = await m_NotifyHttpClient.IsReady();
                Assert.True(isReady);
            }

            if (null == m_NotifySQSClient)
            {
                m_NotifySQSClient = ComponentMgr.Instance.TryResolve<INotificationSQSClient>();
                Assert.NotNull(m_NotifySQSClient);

                var isReady = await m_NotifySQSClient.IsReady();
                Assert.True(isReady);
            }

            return true;
        }

        private void OnConsume(string topic, object data)
        {
            Console.WriteLine($"received from topic(={topic}), msg={data}");
        }

        protected INotificationHttpClient m_NotifyHttpClient = null;
        protected INotificationSQSClient m_NotifySQSClient = null;
    }
}
