using System.Collections.Concurrent;
using Amazon;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Measurement.CloudWatch
{
    public class AwsCloudWatchApmMgr : CObject, IAwsMeasurementMgr
    {
        public IServiceResponse<IMeasurement> GetDefaultService() =>
            GetService(new ConfigOptions<AwsCloudWatch_Option>(
                new AwsCloudWatch_Option()
                {
                    Region = DefaultRegion
                }
            ));

        public IServiceResponse<IMeasurement> GetService(IConfigOptions<AwsCloudWatch_Option> opt)
        {
            var returnValue = new ServiceResponse<IMeasurement>(true);
            var client = m_MeasurementMap.GetOrAdd(opt, (r) =>
            {
                return CreateAWSCloudWatchService(r) as IMeasurement;
            });

            return returnValue.Content(client);
        }

        protected IMeasurement CreateAWSCloudWatchService(IConfigOptions<AwsCloudWatch_Option> opt)
        {
            IMeasurement returnValue = new AwsCloudWatchApm(opt);
            if (returnValue == null)
            {
                Logger.LogError($"Failed to create AWS cloudwatch service. ");
            }

            return returnValue;
        }

        public void Dispose()
        {
            m_MeasurementMap?.Clear();
        }

        public string DefaultRegion { get; set; } = RegionEndpoint.USWest2.SystemName;
        protected ConcurrentDictionary<IConfigOptions<AwsCloudWatch_Option>, IMeasurement> m_MeasurementMap { get; private set; } = new ConcurrentDictionary<IConfigOptions<AwsCloudWatch_Option>, IMeasurement>();
    }
}
