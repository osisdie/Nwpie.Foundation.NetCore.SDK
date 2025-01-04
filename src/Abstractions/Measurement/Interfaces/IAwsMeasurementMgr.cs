using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;

namespace Nwpie.Foundation.Abstractions.Measurement.Interfaces
{
    public interface IAwsMeasurementMgr : ISingleCObject
    {
        /// <summary>
        /// Use DefaultRegion and Environment Variables
        /// </summary>
        /// <returns></returns>
        IServiceResponse<IMeasurement> GetDefaultService();
        IServiceResponse<IMeasurement> GetService(IConfigOptions<AwsCloudWatch_Option> opt);
        string DefaultRegion { get; set; }
    }
}
