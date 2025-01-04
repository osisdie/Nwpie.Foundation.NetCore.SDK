using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Caching.Redis.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Location.Contract;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Base
{
    public class DomainService : DomainServiceBase
    {
        static DomainService()
        {
            m_NotifyClient = ComponentMgr.Instance.TryResolve<IAwsNotificationClient>();
            m_ConfigClient = ComponentMgr.Instance.TryResolve<IConfigClient>();

            // RedisFirst,
            // Alternaive: LocalCache
            m_RedisClient = RedisExtension.GetRedisCacheOrAlternative(
                serviceName: LocationServiceConfig.ServiceName,
                isHealthCheck: true
            );
        }

        public async Task<string> GetApiNameAsync()
        {
            var apiKey = await TokenUtils.ExtractApiKey(CurrentRequest,
                AuthExactFlagEnum.ApiKeyHeader |
                AuthExactFlagEnum.ApiKeyInToken
            );

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
            if (null != profile)
            {
                return profile.SysName
                    ?? string.Concat(profile.Name, ConfigConst.ApiNameDivider, ServiceContext.SdkEnv);
            }

            return null;
        }

        protected async Task BroadCastRefersh<T>(ICommandModel<T> request)
        {
            if (true == ServiceContext.Config.LOC?.BroadLocationEventEnabled &&
                null != m_NotifyClient)
            {
                _ = await m_NotifyClient.BroadcastAsync(request);
            }
        }

        public override ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultCacheClient)
                    {
                        m_DefaultCacheClient = m_RedisClient ?? ComponentMgr.Instance.GetDefaultCache(isFailOverToLocalCache: true);
                    }
                }
            }

            return m_DefaultCacheClient;
        }

        public override IStorage GetStorage()
        {
            if (null == m_DefaultStorageClient)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultStorageClient)
                    {
                        m_DefaultStorageClient = ComponentMgr.Instance.TryResolve<IAwsS3Factory>()
                            .GetDefaultService()?.Data
                            ?? throw new NotSupportedException("Currently storage service unavailable. ");
                    }
                }
            }

            return m_DefaultStorageClient;
        }

        protected static readonly IAwsNotificationClient m_NotifyClient;
        protected static readonly IConfigClient m_ConfigClient;
        protected static readonly ICache m_RedisClient;

        private static readonly object m_Lock = new object();
    }
}
