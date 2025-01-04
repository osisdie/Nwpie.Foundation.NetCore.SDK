using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Notification.Extensions;
using Nwpie.Foundation.MessageQueue.Factory;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Notification.SDK
{
    public class NotificationSQSClient : CObject, INotificationSQSClient
    {
        public NotificationSQSClient(IConfigOptions<AwsSQS_Option> option, ISerializer serializer) : base()
        {
            m_Serializer = serializer ?? throw new ArgumentException(nameof(serializer));
            m_Option = option ?? throw new ArgumentException(nameof(AwsSQS_Option));
            if (null == option.Value)
            {
                throw new ArgumentException(nameof(AwsSQS_Option));
            }

            m_SQSFactory = new MessageQueueFactory(serializer);
        }

        public virtual async Task<IServiceResponse> SendAsync(NotifySend_RequestModel request)
        {
            await IsReady();
            return await m_SQSClient.PublishAsync(request?.PreProcess());
        }

        public virtual async Task<bool> IsReady()
        {
            if (string.IsNullOrWhiteSpace(ServiceContext.ApiName))
            {
                throw new ArgumentNullException(nameof(ServiceContext.ApiName));
            }

            if (string.IsNullOrWhiteSpace(ServiceContext.ApiKey))
            {
                throw new ArgumentNullException(nameof(ServiceContext.ApiKey));
            }

            if (null == m_Option?.Value)
            {
                throw new ArgumentNullException(nameof(AwsSQS_Option));
            }

            var isReady = true;
            if (null == m_SQSClient)
            {
                try
                {
                    m_SQSClient = m_SQSFactory
                        .GetMessageQueue(MessageQueueProviderEnum.SQS, m_Option)
                        ?.Data
                        as IAwsSQSClient;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    isReady = false;
                }
            }

            await Task.CompletedTask;
            return isReady;
        }

        public int DefaultTimeoutSecs { get; set; } = ConfigConst.DefaultHttpTimeout;
        public int DefaultRetries { get; set; } = ConfigConst.DefaultHttpRetry;
        public int DefaultDelayRetrySecs { get; set; } = ConfigConst.DefaultDelayRetrySecs;

        protected readonly IMessageQueueFactory m_SQSFactory;
        protected readonly ISerializer m_Serializer;

        // Load from configserver at runtime
        protected IConfigOptions<AwsSQS_Option> m_Option;
        protected IAwsSQSClient m_SQSClient;
    }
}
