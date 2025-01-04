using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Notification.Lambda.Service.Common
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
    }
}
