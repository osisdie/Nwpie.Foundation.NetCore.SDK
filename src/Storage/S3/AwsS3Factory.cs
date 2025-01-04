using System.Collections.Concurrent;
using Amazon;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Storage.S3
{
    public class AwsS3Factory : CObject, IAwsS3Factory
    {
        public IServiceResponse<IStorage> GetDefaultService(string region = null)
        {
            return GetService(new ConfigOptions<AwsS3_Option>(
                new AwsS3_Option()
                {
                    AccessKey = ServiceContext.AwsProfile?.AccessKey,
                    SecretKey = ServiceContext.AwsProfile?.SecretKey,
                    Region = region ?? DefaultRegion
                }
            ));
        }

        public IServiceResponse<IStorage> GetService(IConfigOptions<AwsS3_Option> opt)
        {
            var returnValue = new ServiceResponse<IStorage>(true);
            var client = StorageMap.GetOrAdd(opt, (r) =>
            {
                return CreateAWSS3Service(r) as IStorage;
            });

            return returnValue.Content(client);
        }

        protected IS3StorageClient CreateAWSS3Service(IConfigOptions<AwsS3_Option> opt)
        {
            IS3StorageClient returnValue = new S3StorageClient(opt, GetCache());
            if (null == returnValue)
            {
                Logger.LogError($"Failed to create AWS file service. ");
            }

            return returnValue;
        }

        public virtual ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                m_DefaultCacheClient = ComponentMgr.Instance.GetDefaultCache(isFailOverToLocalCache: true);
            }

            return m_DefaultCacheClient;
        }

        public void Dispose()
        {
            StorageMap?.Clear();
        }

        public string DefaultRegion { get; set; } = RegionEndpoint.USWest2.SystemName;
        public ConcurrentDictionary<IConfigOptions<AwsS3_Option>, IStorage> StorageMap { get; private set; } = new ConcurrentDictionary<IConfigOptions<AwsS3_Option>, IStorage>();

        protected ICache m_DefaultCacheClient;
    }
}
