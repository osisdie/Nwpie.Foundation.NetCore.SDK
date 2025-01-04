using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Common.Utilities;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Dapper;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Common.Domain
{
    public sealed class ConfigDomainHelper : KvsDataAccessBase
    {
        public static async Task<int> DistanceOfParent(string childApiKey, string parentApiKey, int distance = 0)
        {
            if (null == parentApiKey || childApiKey == parentApiKey)
            {
                return distance;
            }

            var child = await ApiKeyDomainHelper.GetApiKeyByIdAsync(childApiKey);
            if (child.parent_apikey == parentApiKey)
            {
                return distance + 1;
            }

            return await DistanceOfParent(child.parent_apikey, parentApiKey, ++distance);
        }

        public static bool IsNwpieApiKeySet(string apiKey) =>
            true == KvsConfigUtils.PlatformApiKeyList?.Any(x => x.apikey == apiKey);

        public static async Task<bool> IsChildOf(string parentApiKey, string childApiKey, bool includePlatfromLevel = false)
        {
            if (null == parentApiKey)
            {
                return false;
            }

            if (childApiKey == parentApiKey)
            {
                return true;
            }

            var child = await ApiKeyDomainHelper.GetApiKeyByIdAsync(childApiKey);
            if (child.apikey == parentApiKey || child.parent_apikey == parentApiKey)
            {
                return true;
            }

            if (false == includePlatfromLevel && IsNwpieApiKeySet(child.parent_apikey))
            {
                return false;
            }

            return await IsChildOf(parentApiKey, child.parent_apikey);
        }

        public static async Task<List<PERMISSION_CONFIG_KEY_Entity>> ListConfigPermissionAsync()
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "PERMISSION_CONFIG_KEY", "*");
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<PERMISSION_CONFIG_KEY_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<PERMISSION_CONFIG_KEY_Entity>(@"
SELECT *
FROM PERMISSION_CONFIG_KEY
WHERE isdel = 0;");

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<List<PERMISSION_CONFIG_KEY_Entity>> GetConfigPermissionList(string configKey, List<string> appList)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "PERMISSION_CONFIG_KEY", configKey, string.Join(",", appList));
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<PERMISSION_CONFIG_KEY_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<PERMISSION_CONFIG_KEY_Entity>(@"
SELECT *
FROM PERMISSION_CONFIG_KEY
WHERE configkey = @configkey
AND app_id in @list_app_id
AND isdel = 0;",
                    new
                    {
                        configkey = configKey,
                        list_app_id = appList
                    }
                );

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<List<CONFIG_VALUES_Entity>> ListConfigValueAsync()
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_VALUES", "*");
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<CONFIG_VALUES_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_VALUES_Entity>(@"
SELECT *
FROM CONFIG_VALUES
WHERE isdel = 0;");

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<bool?> HasConfigKey(string configKey, List<string> appList)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_KEY", configKey, string.Join(",", appList));
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<bool?>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_VALUES_Entity>(@"
SELECT *
FROM CONFIG_KEY
WHERE configkey = @configkey AND app_id in @list_app_id
AND isdel = 0;",
                    new
                    {
                        configkey = configKey,
                        list_app_id = appList
                    }
                );

                var result = entities?.ToList();
                if (result?.Count > 0 && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, true, CacheDurationSecs);
                }

                return result?.Count() > 0;
            }
        }

        public static async Task<CONFIG_VALUES_Entity> GetLatestValue(string configKey, string appId, string apiKey, bool computeDistance)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                return null;
            }

            var version = ConfigConst.LatestVersion;
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_VALUES", configKey, version, appId, apiKey);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<CONFIG_VALUES_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_VALUES_Entity>(@"
SELECT v.*
FROM CONFIG_VALUES v
WHERE v.configkey = @configkey
AND v.apikey = @apikey
AND v.version = @version
AND v.isdel = 0
AND EXISTS (
    SELECT 1
    FROM CONFIG_KEY k
        INNER JOIN APPLICATION a on k.app_id = a.app_id
    WHERE v.configkey = k.configkey
        AND a.app_id = @app_id
        AND k.isdel = 0 AND a.isdel = 0
);",
                        new
                        {
                            configkey = configKey,
                            apikey = apiKey,
                            version,
                            app_id = appId
                        }
                    );

                var result = entities?.FirstOrDefault();
                if (null != result)
                {
                    if (computeDistance)
                    {
                        result.distance = await DistanceOfParent(result.apikey,
                            KvsConfigUtils.PlatformBaseApiKey
                        );
                    }

                    if (null != CacheClient)
                    {
                        _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                    }
                }

                return result;
            }
        }

        public static async Task<List<CONFIG_VALUES_Entity>> GetValueList(string configKey, string version, List<string> appList, bool computeDistance)
        {
            if (string.IsNullOrWhiteSpace(configKey) ||
                true != (appList?.Count() > 0))
            {
                return null;
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_VALUES", configKey, version, string.Join(",", appList));
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<List<CONFIG_VALUES_Entity>>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_VALUES_Entity>(@"
SELECT v.*
FROM CONFIG_VALUES v
WHERE v.configkey = @configkey AND v.version = @version
AND v.isdel = 0
AND EXISTS (
    SELECT 1
    FROM CONFIG_KEY k
        INNER JOIN APPLICATION a on k.app_id = a.app_id
    WHERE v.configkey = k.configkey
        AND a.app_id in @list_app_id
        AND k.isdel = 0 AND a.isdel = 0
);",
                    new
                    {
                        configkey = configKey,
                        version,
                        list_app_id = appList
                    }
                );

                var result = entities?.ToList();
                if (result?.Count() > 0)
                {
                    if (computeDistance)
                    {
                        result.ForEach(async x =>
                        {
                            x.distance = await
                                DistanceOfParent(
                                    x.apikey,
                                    KvsConfigUtils.PlatformBaseApiKey
                                );
                        });
                    }

                    if (null != CacheClient)
                    {
                        _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                    }
                }

                return result;
            }
        }

        public static async Task<CONFIG_VALUES_Entity> GetLatestValue(string configKey, string appId, string apiKey, string version, bool computeDistance = false)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                return null;
            }

            var apiList = new List<string>() { apiKey };
            var baseKey = await ApiKeyDomainHelper.GetBaseApiKey(apiKey);
            if (baseKey.apikey != apiKey)
            {
                apiList.Add(baseKey.apikey);
            }

            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_VALUES", configKey, version, appId, string.Join(",", apiList));
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<CONFIG_VALUES_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            var sqlCmd = @"
SELECT *
FROM CONFIG_VALUES
WHERE configkey = @configKey
AND app_id = @app_id
AND apikey in @list_apikey
AND version = @version
AND isdel = 0;";

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_VALUES_Entity>(sqlCmd,
                        new
                        {
                            configkey = configKey,
                            app_id = appId,
                            list_apikey = apiList,
                            version = ConfigConst.LatestVersion
                        }
                    );

                var result = entities?.ToList();
                if (result?.Count() > 0)
                {
                    if (computeDistance)
                    {
                        result.ForEach(o =>
                        {
                            o.distance = DistanceOfParent(o.apikey,
                                KvsConfigUtils.PlatformBaseApiKey
                            ).ConfigureAwait(false).GetAwaiter().GetResult();
                        });
                    }

                    CONFIG_VALUES_Entity entity = null;
                    if (result.Any(o => o.apikey == apiKey))
                    {
                        entity = result.Where(o => o.apikey == apiKey).FirstOrDefault();
                    }

                    if (null == entity)
                    {
                        entity = result.FirstOrDefault();
                    }

                    if (null != CacheClient)
                    {
                        _ = await CacheClient.SetAsync(cacheKey, entity, CacheDurationSecs);
                    }

                    return entity;
                }

                return null;
            }
        }

        public static string ValidateVersion(string oldValue, string newValue)
        {
            Version.TryParse(oldValue ?? new Version(1, 0, 0).ToString(), out var oldVer);
            if (newValue.HasValue())
            {
                if (false == newValue.Equals(ConfigConst.LatestVersion, StringComparison.OrdinalIgnoreCase))
                {
                    if (Version.TryParse(newValue, out var newVer) &&
                        newVer >= oldVer)
                    {
                        return newValue;
                    }

                    throw new Exception($"New version (={newValue}) less than existing version (={oldValue}). ");
                }
                else
                {
                    return new Version(oldVer.Major, oldVer.Minor, oldVer.Build + 1).ToString();
                }
            }
            else
            {
                return new Version(1, 0, 0).ToString();
            }
        }

        public static async Task<CONFIG_KEY_Entity> NewConfigKey(string ownerAppId, KvsSet_RequestModel data)
        {
            var sqlCmd = @"
INSERT IGNORE INTO CONFIG_KEY (configkey, app_id, section, encrypted)
SELECT @configkey,
  @app_id,
  @section,
  @encrypted;

SELECT ROW_COUNT();";

            using (var conn = GetMySqlConnection())
            {
                var result = await conn.ExecuteScalarAsync<int>(sqlCmd,
                    new
                    {
                        app_id = ownerAppId,
                        configKey = data.ConfigKey,
                        section = data.ConfigSection,
                        encrypted = data.NeedEncrypt
                    }
                );
                if (result > 0)
                {
                    _ = CacheClient?.RemovePatternAsync(DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_KEY", data.ConfigKey));
                    return await
                        GetConfigKey(data.ConfigKey, ownerAppId);
                }
            }

            return null;
        }

        public static async Task<CONFIG_KEY_Entity> GetConfigKey(string configKey, string ownerAppId)
        {
            var cacheKey = DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_KEY", configKey, ownerAppId);
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<CONFIG_KEY_Entity>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            using (var conn = GetMySqlConnection())
            {
                var entities = await conn
                    .QueryAsync<CONFIG_KEY_Entity>(@"
SELECT *
FROM CONFIG_KEY
WHERE configkey = @configkey AND app_id = @app_id
AND isdel = 0;",
                    new
                    {
                        app_id = ownerAppId,
                        configkey = configKey
                    }
                );

                var result = entities?.FirstOrDefault();
                if (null != result && null != CacheClient)
                {
                    _ = await CacheClient.SetAsync(cacheKey, result, CacheDurationSecs);
                }

                return result;
            }
        }

        public static async Task<CONFIG_VALUES_Entity> SetConfig(string ownerAppId, string apiKey, KvsSet_RequestModel data)
        {
            var currentLatest = await GetLatestValue(data.ConfigKey,
                ownerAppId,
                apiKey,
                ConfigConst.LatestVersion);

            var sqlCmd = @"
INSERT INTO CONFIG_VALUES(configkey, app_id, apikey, version, version_display, rawdata, encrypted)
SELECT @configkey, @app_id, @apikey, @version_display, @version_display, @rawdata, @encrypted;

UPDATE CONFIG_KEY
SET latest_value_updated = now()
WHERE configkey = @configkey
AND app_id = @app_id
AND isdel = 0;

SELECT ROW_COUNT();";

            data.VersionDisplay = ValidateVersion(currentLatest?.version_display, data.VersionDisplay);
            if (null == currentLatest)
            {
                sqlCmd = @"
INSERT INTO CONFIG_VALUES(configkey, app_id, apikey, version, version_display, rawdata, encrypted)
SELECT @configkey, @app_id, @apikey, @version, @version_display, @rawdata, @encrypted
;" + sqlCmd;
            }
            else
            {
                sqlCmd = @"
INSERT INTO CONFIG_VALUES(configkey, app_id, apikey, version, version_display, rawdata, encrypted)
SELECT @configKey, @app_id, @apiKey, @version, @version_display, @rawdata, @encrypted
ON DUPLICATE KEY UPDATE
    version_display = @version_display,
    rawdata = @rawdata,
    encrypted = @encrypted
;" + sqlCmd;
            }

            using (var conn = GetMySqlConnection())
            {
                var result = await conn.ExecuteScalarAsync<int>(sqlCmd,
                    new
                    {
                        app_id = ownerAppId,
                        configKey = data.ConfigKey,
                        apikey = apiKey,
                        version = ConfigConst.LatestVersion,
                        version_display = data.VersionDisplay,
                        rawdata = data.RawData,
                        encrypted = data.NeedEncrypt
                    }
                );
                if (result > 0)
                {
                    if (null != CacheClient)
                    {
                        await CacheClient.RemovePatternAsync(DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "CONFIG_VALUES", data.ConfigKey)).ConfigureAwait(false);
                    }

                    return await GetLatestValue(data.ConfigKey, ownerAppId, apiKey, ConfigConst.LatestVersion);
                }
            }

            return null;
        }

        public static async Task<int?> SetPermission(string configKey, PERMISSION_CONFIG_KEY_Entity param)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                return null;
            }

            using (var conn = GetMySqlConnection())
            {
                var result = await conn
                    .ExecuteScalarAsync<int>(@"
INSERT INTO PERMISSION_CONFIG_KEY (configkey, app_id, target_id, target_type, perm_id, weight, blocked)
SELECT @configkey, @app_id, @target_id, @target_type, @perm_id, @weight, @blocked
WHERE NOT EXISTS (
    SELECT 1
    FROM PERMISSION_CONFIG_KEY p2
    WHERE p2.configkey = @configkey
        AND p2.app_id = @app_id
        AND p2.target_id = @target_id
        AND p2.target_type = @target_type
        AND p2.perm_id = @perm_id
);

SELECT ROW_COUNT();", new
                    {
                        configkey = configKey,
                        param.app_id,
                        param.target_id,
                        param.target_type,
                        param.perm_id,
                        blocked = param.blocked ?? false,
                        weight = param.weight ?? 1
                    });

                if (result > 0)
                {
                    _ = CacheClient?.RemovePatternAsync(DataAccessUtils.GetTableCacheKey(KvsConst.DefaultDatabaseName, "PERMISSION_CONFIG_KEY", configKey));
                }

                return result;
            }
        }

        private const int CacheDurationSecs = 4 * 60 * 60;
    }
}
