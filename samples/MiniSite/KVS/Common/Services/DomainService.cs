using System;
using System.Threading.Tasks;
using Autofac;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Http.Common.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.Common.Models;

namespace Nwpie.MiniSite.KVS.Common.Services
{
    public class DomainService : DomainServiceBase
    {
        public async Task<KvsApiKey> GetAccessApiKey(RequestDtoBase param)
        {
            var kvs = new KvsApiKey();
            var apiKeyInHeader = Headers?[CommonConst.ApiKey];
            if (true == apiKeyInHeader?.HasValue())
            {
                return await kvs.FillByApiKey(apiKeyInHeader);
            }

            if (null != ServiceEntry)
            {
                var tokenDetail = await ServiceEntry.GetTokenDetail(AuthExactFlagEnum.ApiKeyHeader);
                if (true == tokenDetail?.ApiKey?.HasValue())
                {
                    return await kvs.FillByApiKey(apiKeyInHeader);
                }
            }

            throw new ArgumentNullException($"Missing {nameof(CommonConst.ApiKey)}. ");
        }

        public override T GetRepository<T>(bool isSelfService = false)
        {
            try
            {
                return (T)ComponentMgr.Instance.DIContainer
                    .Resolve(typeof(T),
                        new TypedParameter(
                            typeof(IRequestService),
                            ServiceEntry ?? new EmptyRequestService()
                        )
                    );
            }
            catch (Exception ex)
            {
                throw new Exception("Missing *_Repository", ex);
            }
        }

        public override T GetDomainService<T>(bool isSelfService = false)
        {
            try
            {
                return (T)ComponentMgr.Instance.DIContainer
                    .Resolve(typeof(T),
                        new TypedParameter(
                            typeof(IRequestService),
                            ServiceEntry ?? new EmptyRequestService()
                        )
                    );
            }
            catch (Exception ex)
            {
                throw new Exception("Missing *_DomainService", ex);
            }
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
    }
}
