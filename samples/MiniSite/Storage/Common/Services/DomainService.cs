using System;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.MiniSite.Storage.Common.Services
{
    public class DomainService : DomainServiceBase
    {
        public override ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                m_DefaultCacheClient = ComponentMgr.Instance.GetDefaultCache(isFailOverToLocalCache: true);
            }

            return m_DefaultCacheClient;
        }

        public override IStorage GetStorage()
        {
            if (null == m_DefaultStorageClient)
            {
                m_DefaultStorageClient = ComponentMgr.Instance.TryResolve<IAwsS3Factory>()
                    .GetDefaultService()?.Data
                    ?? throw new NotSupportedException("Currently storage service unavailable. ");
            }

            return m_DefaultStorageClient;
        }
    }
}
