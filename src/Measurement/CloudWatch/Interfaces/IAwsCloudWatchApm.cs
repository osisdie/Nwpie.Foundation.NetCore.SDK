using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Measurement.Enums;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Measurement.CloudWatch.Interfaces
{
    public interface IAwsCloudWatchApm : IMeasurement
    {
        Task<bool> WriteAsync(string metricName, List<KeyValuePair<MeasurementUnitEnum, double>> metrics, Dictionary<string, string> dimensions = null, string category = CommonConst.SdkPrefix);
        Task<bool> WriteAsync(Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>> metrics, Dictionary<string, string> dimensions = null, string category = CommonConst.SdkPrefix);

    }
}
