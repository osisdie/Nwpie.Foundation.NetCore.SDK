using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Measurement;
using Nwpie.Foundation.Common.MessageQueue;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Measurement.SDK.Interfaces;

namespace Nwpie.Foundation.Measurement.SDK
{
    /// <summary>
    /// Util class supports measurement service REST API.
    /// </summary>
    internal sealed class MeasurementHost : DefaultMeasurementHost, IMeasurementHost
    {
        public override void InitialClient()
        {
            DefaultTopic = m_Option.Topic;

            var amazonSQSConfig = new AmazonSQSConfig
            {
                ServiceURL = m_Option.ServiceUrl
            };

            if (m_Option.AccessKey.HasValue() &&
                m_Option.SecretKey.HasValue())
            {
                m_SQSClient = new AmazonSQSClient(m_Option.AccessKey, m_Option.SecretKey, amazonSQSConfig);
            }
            else
            {
                m_SQSClient = new AmazonSQSClient(amazonSQSConfig);
            }
        }

        public override async Task PushPointsToServer(List<MetricPoint> points)
        {
            // Also can add extra metrics here
            var requestBody = new MeasurementRequest()
            {
                DBName = DefaultDBName,
                MetricPoints = points
            };
            //requestBody.MetricPoints.AddRange(points);

            var requestToJsonString = m_Serializer.Serialize(requestBody);
            var queueUrl = $"{m_Option.QueueBaseUrl.TrimEndSlash()}/{m_Option.Topic}";
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

            if (m_Option.Topic.EndsWith(MessageQueueConst.FIFO_Suffix, StringComparison.OrdinalIgnoreCase))
            {
                // ContentBase FIFO
                sqsRequest.MessageGroupId = ServiceContext.ApiName;
                sqsRequest.MessageDeduplicationId = CryptoUtils.GetSha256String(requestToJsonString);
            }

            var response = await m_SQSClient.SendMessageAsync(sqsRequest);
#if DEBUG
            if (false == IsSilent)
            {
                Console.WriteLine($"[{(string.IsNullOrWhiteSpace(response?.MessageId) ? "Error" : "OK")}] MeasurementHost: Sending {points.Count} point(s) to TSDB '{requestBody.DBName}' via {m_Option.Topic}. ");
            }
#endif
        }

        public override void Dispose() { }
    }
}
