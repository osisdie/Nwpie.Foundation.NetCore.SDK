using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.MiniSite.KVS.Common.Entities.Application;
using Dapper;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Common.Domain
{
    public sealed class ApplicationDomainHelper : KvsDataAccessBase
    {
        public static void RemoveCachePattern(string pattern)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, pattern);
            _ = CacheClient?.RemovePatternAsync(cacheKey);
        }

        public static async Task<APPLICATION_Entity> GetApplicationByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, id);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<APPLICATION_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            var list = await ListApplicationAsync();
            var result = list?.FirstOrDefault(o => o.app_id == id);
            if (null != result && null != CacheClient)
            {
                _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
            }

            return result;
        }

        public static async Task<APPLICATION_Entity> GetApplicationByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, name);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<APPLICATION_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            var list = await ListApplicationAsync();
            var result = list?.FirstOrDefault(o => o.sys_name == name);
            if (null != result && null != CacheClient)
            {
                _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
            }

            return result;
        }

        public static async Task<List<APPLICATION_Entity>> ListApplicationAsync()
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, "*");
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<APPLICATION_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<APPLICATION_Entity>(@"
SELECT *
FROM APPLICATION
WHERE isdel = 0;");

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        private const string TableName = "APPLICATION";
        private const int CacheDurationSecs = 4 * 60 * 60;
    }
}
