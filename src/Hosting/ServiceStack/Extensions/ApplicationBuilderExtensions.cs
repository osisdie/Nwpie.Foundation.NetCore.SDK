using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.MessageQueue.Enums;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Hosting.ServiceStack.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        static ApplicationBuilderExtensions()
        {
            Logger = LogMgr.CreateLogger(typeof(ApplicationBuilderExtensions));
            m_Serializer = new DefaultSerializer();
            m_FailbackScore = FailbackScoreInfo.CreateNew;

            if (ServiceContext.ListenLocationEventEnabled &&
                null != SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey.ConfigServerRawValue())
            {
                try
                {
                    m_SQSFactory = ComponentMgr.Instance.TryResolve<IMessageQueueFactory>();
                    m_SNSClient = ComponentMgr.Instance.TryResolve<IAwsNotificationClient>();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.ToString());
                }
            }
        }

        public static IApplicationBuilder RegisterMyLocation(this IApplicationBuilder app,
            string serviceName,
            IHostApplicationLifetime lifetime = null)
        {
            m_Lifetime = lifetime;

            try
            {
                Task.Delay(DefaultConsumeInterval * 2)
                    .ContinueWith(t =>
                    {
                        app.RegisterToSNS(serviceName);
                    });
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return app;
        }

        public static IApplicationBuilder RegisterEvent(this IApplicationBuilder app,
            string key,
            Action<string, ICommandModel> callback)
        {
            if (m_CallbackEvents.ContainsKey(key))
            {
                m_CallbackEvents[key].Add(callback);
            }
            else
            {
                m_CallbackEvents.Add(key, new List<Action<string, ICommandModel>> { callback });
            }

            return app;
        }

        public static void RegisterToSNS(this IApplicationBuilder app, string serviceName)
        {
            if (null == m_SNSClient || null == m_SQSFactory || serviceName.IsNullOrEmpty())
            {
                return;
            }

            var topic = Location.Contract.LocationServiceConfig
                .CreateQueueName(ServiceContext.ApiName,
                    ServiceContext.MachineName
                );

            if (null == ServiceContext.Config.LOC)
            {
                ServiceContext.Config.LOC = new LocationConfigurationSection();
            }

            if (null == ServiceContext.Config.LOC.SQS)
            {
                ServiceContext.Config.LOC.SQS = new AwsSQS_Option();
            }

            ServiceContext.Config.LOC.SQS.QueueUrl = m_SNSClient
                .GetQueueUrlOrAddNewQueue(topic).ConfigureAwait(false).GetAwaiter().GetResult()
                ?.Data;

            if (true == ServiceContext.Config.LOC.SQS.QueueUrl.IsNullOrEmpty())
            {
                return;
            }

            ServiceContext.Config.LOC.SQS.QueueArn = m_SNSClient
                .LookupArn(ServiceContext.Config.LOC.SQS.QueueUrl).ConfigureAwait(false).GetAwaiter().GetResult()
                ?.Data;

            if (ServiceContext.Config.LOC.SQS.QueueArn.IsNullOrEmpty())
            {
                return;
            }

            ServiceContext.Config.LOC.SQS.SubscriptionArn = m_SNSClient
                .SubscribeAsync(ServiceContext.Config.LOC.SQS.QueueArn).ConfigureAwait(false).GetAwaiter().GetResult()
                ?.Data;

            if (ServiceContext.Config.LOC.SQS.SubscriptionArn.IsNullOrEmpty())
            {
                return;
            }

            var sqsOpt = SysConfigKey
                .Default_AWS_SQS_Urls_Notification_ConfigKey
                .ConfigServerValue<AwsSQS_Option>();
            if (null != sqsOpt)
            {
                var opt = new ConfigOptions<AwsSQS_Option>(
                    new AwsSQS_Option()
                    {
                        ServiceUrl = sqsOpt.ServiceUrl,
                        QueueBaseUrl = sqsOpt.QueueBaseUrl,
                        Topic = topic
                    }
                );

                m_SQS_ListeningSNS = m_SQSFactory
                    .GetMessageQueue(MessageQueueProviderEnum.SQS, opt)
                    ?.Data
                    as IAwsSQSClient;

                if (null != m_SQS_ListeningSNS &&
                    m_CallbackEvents.ContainsKey(LocationConst.LocationEventName))
                {
                    foreach (var callback in m_CallbackEvents[LocationConst.LocationEventName])
                    {
                        m_SQS_ListeningSNS.ConsumedEvent += callback;
                        Logger.LogInformation($"Registered SNS ConsumedEvent event for callback: {callback.Method.Name}");
                    }
                }

                // start timer
                StartRepeatConsumeQueueTimer();
            }

            if (null == m_Lifetime)
            {
                return;
            }

            m_Lifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    if (null != m_SNSClient &&
                        true == ServiceContext.Config?.LOC?.SQS?.QueueArn.HasValue())
                    {
                        m_SNSClient.UnsubscribeAsync(ServiceContext.Config.LOC.SQS.QueueArn);

                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.ToString());
                }
            });
        }

        private static void StartRepeatConsumeQueueTimer()
        {
            m_Timer = new Timer(TimerCallback,
                null,
                m_ConsumeInterval,
                Timeout.Infinite
            );
        }

        private static void RestartTimer()
        {
            m_Timer?.Change(m_ConsumeInterval, Timeout.Infinite);
        }

        private static void TimerCallback(object state)
        {
            if (null == m_SQS_ListeningSNS || m_FailbackScore.IsExceedLimit())
            {
                return;
            }

            try
            {
                var result = m_SQS_ListeningSNS.ConsumeAsync<string>().ConfigureAwait(false).GetAwaiter().GetResult();

                m_FailbackScore.Score(result.IsSuccess);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
                m_FailbackScore.Fail();
            }

            RestartTimer();
        }

        public const int DefaultConsumeInterval = 15000;

        private static readonly ILogger Logger;
        private static readonly ISerializer m_Serializer;
        private static readonly IAwsNotificationClient m_SNSClient;
        private static readonly IMessageQueueFactory m_SQSFactory;
        private static readonly IFailbackScore m_FailbackScore;
        private static readonly Dictionary<string, List<Action<string, ICommandModel>>> m_CallbackEvents = new Dictionary<string, List<Action<string, ICommandModel>>>(StringComparer.OrdinalIgnoreCase);
        private static IAwsSQSClient m_SQS_ListeningSNS;
        private static IHostApplicationLifetime m_Lifetime;
        private static readonly int m_ConsumeInterval = DefaultConsumeInterval;
        private static Timer m_Timer;
    }
}
