using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Contract.Enums;
using Dapper;

namespace Nwpie.MiniSite.KVS.Common.Domain
{
    public sealed class ApiKeyDomainHelper : KvsDataAccessBase
    {
        public static void RemoveCachePattern(string pattern)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, pattern);
            _ = CacheClient?.RemovePatternAsync(cacheKey);
        }

        [Obsolete("Use TokenProfileMgr")]
        public static async Task<List<API_KEY_Entity>> ListApiKeyAsync(string appId)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, appId);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<API_KEY_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<API_KEY_Entity>(@"
SELECT *
FROM API_KEY
WHERE app_id = @app_id
AND isdel = 0;",
                    new { app_id = appId }
                );

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<API_KEY_Entity> GetBaseApiKeyAsync(string appId)
        {
            var list = await ListApiKeyAsync(appId);
            var @base = list?.Where(x => x.env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Testing))?.FirstOrDefault();
            if (null != @base)
            {
                return @base;
            }

            return list?.FirstOrDefault();
        }

        public static async Task<List<API_KEY_Entity>> ListApiKeyAsync()
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, "*");
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<API_KEY_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<API_KEY_Entity>(@"
SELECT *
FROM API_KEY
WHERE isdel = 0;");

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<API_KEY_Entity> GetBaseApiKey(string apiKey)
        {
            var api = await GetApiKeyByIdAsync(apiKey);
            return await GetBaseApiKeyByAppAsync(api?.app_id);
        }

        public static async Task<List<API_KEY_Entity>> ListApiKeyByAppAsync(string appId)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, appId);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<API_KEY_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<API_KEY_Entity>(@"
SELECT *
FROM API_KEY
WHERE app_id = @app_id
AND isdel = 0;",
                    new { app_id = appId }
                );

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        [Obsolete]
        public static async Task<API_KEY_Entity> GetBaseApiKeyByAppAsync(string appId) =>
            await GetApiKeyByIdAsync(appId);

        [Obsolete]
        public static async Task<API_KEY_Entity> GetApiKeyByIdAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, apiKey);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<API_KEY_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<API_KEY_Entity>(@"
SELECT *
FROM API_KEY
WHERE apikey = @apikey;",
                    new { apikey = apiKey }
                );

                var result = entities?.FirstOrDefault();
                if (null != result && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<API_KEY_Entity> GetApiKeyByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, TableName, name);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<API_KEY_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<API_KEY_Entity>(@"
SELECT *
FROM API_KEY
WHERE sys_name = @sys_name;",
                    new { sys_name = name }
                );

                var result = entities?.FirstOrDefault();
                if (null != result && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        #region status
        public static bool IsValid(string status) =>
            status == Enum<KvsApiKeyStatusEnum>.GetDispValue(KvsApiKeyStatusEnum.Active);

        public static bool IsInvalid(string status) =>
            status == Enum<KvsApiKeyStatusEnum>.GetDispValue(KvsApiKeyStatusEnum.InActive);

        #endregion

        #region converter
        public static string ConvertTo_Valid_StatusCode() =>
            Enum<KvsApiKeyStatusEnum>.GetDispValue(KvsApiKeyStatusEnum.Active);
        public static string ConvertTo_Invalid_StatusCode() =>
            Enum<KvsApiKeyStatusEnum>.GetDispValue(KvsApiKeyStatusEnum.InActive);

        #endregion

        private const string TableName = "API_KEY";
        private const int CacheDurationSecs = 4 * 60 * 60;
    }
}
