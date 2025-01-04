using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Measurement;
using Nwpie.Foundation.Common.MessageQueue;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Nwpie.Foundation.Measurement.SDK.Interfaces;
using Nwpie.Foundation.MessageQueue.SQS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Measurement.SDK
{
    /// <summary>
    /// Util class supports measurement service Rest API.
    /// </summary>
    public class DefaultMeasurementHost : CObject, IMeasurementHost
    {
        public DefaultMeasurementHost()
        {
            m_Serializer = new DefaultSerializer();
            m_WaitSentQueue = new WorkerQueue<MetricPoint>(
                (ServiceContext.Config.APM?.MaxMessageCount ?? 0).AssignIfNotSet(1000),
                (ServiceContext.Config.APM?.SendFrequency ?? 0).AssignIfNotSet(1000),
                QueueName
            );

            MeasurementEnabled = ServiceContext.MeasurementTraceEnabled;
            IsSilent = ServiceContext.Config.APM?.IsSilent
                ?? false;
            DefaultDBName = ServiceContext.Config.APM?.InfluxDefaultDB;

            var sqsOption = SysConfigKey
                .Default_AWS_SQS_Urls_Measurement_ConfigKey
                .ConfigServerRawValue();
            if (sqsOption.HasValue())
            {
                try
                {
                    m_Option = JsonConvert.DeserializeObject<AwsSQS_Option>(sqsOption);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }

            // Pure http
            if (MeasurementEnabled && null == m_Option)
            {
                using (var configService = new DefaultRemoteConfigClient(new DefaultSerializer()))
                {
                    var response = configService.GetLatest<AwsSQS_Option>(
                            SysConfigKey.Default_AWS_SQS_Urls_Measurement_ConfigKey
                        ).ConfigureAwait(false).GetAwaiter().GetResult();

                    m_Option = response?.Data;
                }
            }

            if (MeasurementEnabled &&
                ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), m_Option?.ServiceUrl, m_Option?.QueueBaseUrl, m_Option?.Topic))
            {
                Console.WriteLine($@"[Error] Options
                    {nameof(AwsSQS_Option.ServiceUrl)},
                    {nameof(AwsSQS_Option.QueueBaseUrl)},
                    {nameof(AwsSQS_Option.Topic)} are required. ");

                MeasurementEnabled = false;
            }

            if (MeasurementEnabled)
            {
                InitialClient();
                m_WaitSentQueue.Flush += ProcessPoints;
            }
        }

        public virtual void InitialClient()
        {
            DefaultTopic = m_Option.Topic;

            m_MessageQueueClient = new AwsSQSClient(
                new ConfigOptions<AwsSQS_Option>(m_Option),
                m_Serializer
           );
        }

        public virtual void PerformWrite(MetricPoint point)
        {
            if (MeasurementEnabled && true == point?.Name.HasValue())
            {
                m_WaitSentQueue.Enqueue(point);
            }
        }

        public virtual void ProcessPoints(object sender, List<MetricPoint> points)
        {
            try
            {
                if (points?.Count() > 0)
                {
                    _ = PushPointsToServer(points);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }
        }

        public virtual async Task PushPointsToServer(List<MetricPoint> points)
        {
            // Also can add extra metrics here
            var requestBody = new MeasurementRequest()
            {
                DBName = DefaultDBName,
                MetricPoints = points
            };
            //requestBody.MetricPoints.AddRange(points);

            if (null != m_MessageQueueClient)
            {
                var response = await m_MessageQueueClient.PublishAsync(m_Option.Topic, requestBody);
#if DEBUG
                if (false == IsSilent)
                {
                    Console.WriteLine($"[{(true == response?.IsSuccess ? "OK" : "Error")}] MeasurementHost: Sending {points.Count} point(s) to TSDB '{requestBody.DBName}' via {m_Option.Topic}. ");
                }
#endif
            }
        }

        public virtual void Dispose()
        {
            m_WaitSentQueue?.Dispose();
            m_MessageQueueClient?.Dispose();
        }

        public string DefaultTopic { get; set; }
        public string DefaultDBName { get; set; }
        public bool MeasurementEnabled { get; set; }
        public string QueueName { get; set; } = "MeasurementHost.WorkerQueue";
        public bool IsSilent { get; set; }

        protected readonly WorkerQueue<MetricPoint> m_WaitSentQueue;
        protected readonly AwsSQS_Option m_Option;
        protected readonly ISerializer m_Serializer;

        protected AmazonSQSClient m_SQSClient;
        protected IMessageQueue m_MessageQueueClient;
    }
}
