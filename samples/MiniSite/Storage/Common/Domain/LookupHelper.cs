using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Auth.Contract.Enums.Kind;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Lookup.ListOptions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Microsoft.Extensions.Logging;

namespace Nwpie.MiniSite.Storage.Common.Domain
{
    public sealed class LookupHelper
    {
        static LookupHelper()
        {
            Logger = LogMgr.CreateLogger(typeof(LookupHelper));
            CacheClient = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
        }

        public static string GetPermissionId(PermissionLevelEnum permission) =>
            GetPermissionId(permission.GetDisplayName());

        public static string GetPermissionId(string sysName)
        {
            var permissions = ListLookupAsync(AuthLookupCategoryEnum.AssetPermission.GetDisplayName()).ConfigureAwait(false).GetAwaiter().GetResult();
            if (true == permissions?.Any(o => sysName == o.Name))
            {
                return permissions.First(o => sysName == o.Name).Value;
            }

            return null;
        }

        public static async Task<IEnumerable<LkpListOptions_ResponseModelItem>> ListLookupAsync(string category)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(AuthServiceConfig.DefaultDatabaseName, "LOOKUP", category);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<LkpListOptions_ResponseModelItem>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            try
            {
                var request = new LkpListOptions_Request
                {
                    Data = new LkpListOptions_RequestModel
                    {
                        Category = category,
                        PageIndex = ConfigConst.MinPageIndex,
                        PageSize = ConfigConst.MaxPageSize
                    }
                };

                var response = await request.InvokeAsyncByBaseUrl<LkpListOptions_Response>(
                    baseUrl: ServiceContext.AuthServiceUrl,
                    headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
                );

                var items = response?.Data?.Items;
                if (items?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, items, CacheDurationSecs);
                }

                return items;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return null;
        }

        private const int CacheDurationSecs = 8 * 60 * 60;
        private static readonly ILogger Logger;
        private static readonly ICache CacheClient;
    }
}
