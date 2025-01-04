using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Measurement.Enums;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Measurement.CloudWatch.Interfaces;

namespace Nwpie.Foundation.Measurement.CloudWatch
{
    [Obsolete("Not cheap solution. Use InfluxDBApm instead.")]
    public class AwsCloudWatchApm : CObject, IAwsCloudWatchApm
    {
        public AwsCloudWatchApm(IConfigOptions<AwsCloudWatch_Option> option)
        {
            m_Option = option
                ?? throw new ArgumentNullException(nameof(IConfigOptions<AwsCloudWatch_Option>));
            AwsRegionEndpoint = RegionEndpoint.GetBySystemName(m_Option.Value.Region);

            m_Client = new AmazonCloudWatchClient(AwsRegionEndpoint);
        }

        public async Task<bool> WriteAsync(string metricName, List<KeyValuePair<MeasurementUnitEnum, double>> metrics, Dictionary<string, string> dimensions, string category = CommonConst.SdkPrefix)
        {
            return await WriteAsync(new Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>>(StringComparer.OrdinalIgnoreCase)
                { { metricName, metrics } },
                dimensions,
                category
            );
        }

        public async Task<bool> WriteAsync(Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>> metrics, Dictionary<string, string> dimensions, string category = CommonConst.SdkPrefix)
        {
            dimensions = dimensions ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dimensions.Add(CommonConst.ApiName, ServiceContext.ApiName);

            var data = new List<MetricDatum>();
            foreach (var item in metrics)
            {
                var metricName = item.Key;
                foreach (var metric in item.Value)
                {
                    data.Add(new MetricDatum()
                    {
                        MetricName = metricName,
                        Unit = ConvertToStandardUnit(metric.Key),
                        Value = metric.Value,
                        TimestampUtc = DateTime.UtcNow,
                        Dimensions = dimensions.Select(x =>
                            new Dimension()
                            {
                                Name = x.Key,
                                Value = x.Value
                            }
                        ).ToList()
                    });
                }
            }

            var result = await m_Client.PutMetricDataAsync(new PutMetricDataRequest()
            {
                MetricData = data,
                Namespace = category
            });

            return result.HttpStatusCode == HttpStatusCode.OK;
        }

        public void WritePoint(string metric, string key, double value) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            });

        public void WritePoint(string metric, IDictionary<string, double> fields)
        {
            var adapterDict = new Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in fields)
            {
                if (adapterDict.ContainsKey(item.Key))
                {
                    continue;
                }

                adapterDict[item.Key] = new List<KeyValuePair<MeasurementUnitEnum, double>>()
                {
                    new KeyValuePair<MeasurementUnitEnum, double>(ConvertToMeasurementUnit(item.Value), item.Value)
                };
            }

            _ = WriteAsync(adapterDict, null);
        }

        public void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags) =>
            WritePoint(metric, fields);

        public void WritePoint(string metric, string key, double value, string tagKey, string tagValue) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            });

        public void WritePoint(string metric, string key, double value, IDictionary<string, string> tags) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            });

        public void WritePoint(string metric, IDictionary<string, double> fields, string tagKey, string tagValue) =>
            WritePoint(metric, fields);

        public void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags, DateTime timestampInUTC) =>
            WritePoint(metric, fields);

        private StandardUnit ConvertToStandardUnit(MeasurementUnitEnum unit)
        {
            switch (unit)
            {
                case MeasurementUnitEnum.Bits:
                    return StandardUnit.Bits;
                case MeasurementUnitEnum.Count:
                    return StandardUnit.Count;
                case MeasurementUnitEnum.Percent:
                    return StandardUnit.Percent;
                case MeasurementUnitEnum.Seconds:
                    return StandardUnit.Seconds;
                case MeasurementUnitEnum.Bytes:
                    return StandardUnit.Bytes;
                default:
                    return StandardUnit.None;
            };
        }

        private MeasurementUnitEnum ConvertToMeasurementUnit(double value)
        {
            if (value == Math.Floor(value))
            {
                return MeasurementUnitEnum.Count;
            }

            return MeasurementUnitEnum.Percent;
        }

        public Amazon.RegionEndpoint AwsRegionEndpoint { get; set; }

        protected IConfigOptions<AwsCloudWatch_Option> m_Option;
        protected IAmazonCloudWatch m_Client;
    }
}
