using System.Collections.Concurrent;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;

namespace Nwpie.Foundation.Common.Storage.Interfaces
{
    public interface IAwsS3Factory : ISingleCObject
    {
        /// <summary>
        /// Use DefaultRegion and Environment Variables
        /// </summary>
        /// <returns></returns>
        IServiceResponse<IStorage> GetDefaultService(string region = null);
        IServiceResponse<IStorage> GetService(IConfigOptions<AwsS3_Option> opt);
        ICache GetCache();

        /// <summary>
        /// (regionName, singleton)
        /// </summary>
        ConcurrentDictionary<IConfigOptions<AwsS3_Option>, IStorage> StorageMap { get; }
        string DefaultRegion { get; set; }
    }
}
