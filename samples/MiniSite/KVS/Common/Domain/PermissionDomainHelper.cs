using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.MiniSite.KVS.Common.Entities.Permission;
using Dapper;

namespace Nwpie.MiniSite.KVS.Common.Domain
{
    public sealed class PermissionDomainHelper : KvsDataAccessBase
    {
        public static void RemoveCachePattern(string pattern)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, pattern);
            _ = CacheClient?.RemovePatternAsync(cacheKey);
        }

        public static async Task<List<PERMISSION_Entity>> ListPermissionAsync()
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<PERMISSION_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<PERMISSION_Entity>(@"
SELECT *
FROM PERMISSION
WHERE isdel = 0;");

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result.ToList(), CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<PERMISSION_Entity> GetPermissionByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, id);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<PERMISSION_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            var list = await ListPermissionAsync();
            var result = list?.FirstOrDefault(o => o.perm_id == id);
            if (null != result && null != CacheClient)
            {
                _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
            }

            return result;
        }

        public static async Task<PERMISSION_Entity> GetPermissionByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, name);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<PERMISSION_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            var list = await ListPermissionAsync();
            var result = list?.FirstOrDefault(o => o.sys_name == name);
            if (null != result && null != CacheClient)
            {
                _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
            }

            return result;
        }

        private const string TableName = "PERMISSION";
        private const int CacheDurationSecs = 4 * 60 * 60;
    }
}
