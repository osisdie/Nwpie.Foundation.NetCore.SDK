using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Notification.Endpoint.Common
{
    public class DomainService : DomainServiceBase
    {
        static DomainService()
        {
            m_NotifyClient = ComponentMgr.Instance.TryResolve<IAwsSQSClient>();
        }

        public override ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                m_DefaultCacheClient = ComponentMgr.Instance
                    .GetDefaultCache(isFailOverToLocalCache: true);
            }

            return m_DefaultCacheClient;
        }

        protected static readonly IAwsSQSClient m_NotifyClient;
    }
}
