using System;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.Foundation.Storage.S3.Interfaces;

namespace Nwpie.Foundation.S3Proxy.Endpoint.Common
{
    public class DomainService : DomainServiceBase
    {
        public override ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                m_DefaultCacheClient = ComponentMgr.Instance
                    .GetDefaultCache(isFailOverToLocalCache: true);
            }

            return m_DefaultCacheClient;
        }

        public override IStorage GetStorage()
        {
            if (null == m_DefaultStorageClient)
            {
                m_DefaultStorageClient = ComponentMgr.Instance.TryResolve<IS3StorageClient>()
                    ?? throw new NotSupportedException("Currently storage service unavailable. ");
            }

            return m_DefaultStorageClient;
        }
    }
}
