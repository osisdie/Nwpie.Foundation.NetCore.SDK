using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Cache.Measurement;
using Nwpie.Foundation.Common.Serializers;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace Nwpie.Foundation.Caching.Redis
{
    public class RedisCache : CacheBase, IRedisCache
    {
        public RedisCache(IConfigOptions<RedisCache_Option> option)
            : base(option)
        {
        }

        protected override void Initialization()
        {
            string connStr = (m_Option as IConfigOptions<RedisCache_Option>)?.Value?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new ArgumentNullException(nameof(RedisCache_Option.ConnectionString));
            }

            var cfgOpt = ConfigurationOptions.Parse(
                ((IConfigOptions<RedisCache_Option>)m_Option).Value.ConnectionString
            );
            cfgOpt.AbortOnConnectFail = false;
            cfgOpt.AllowAdmin = true;
            cfgOpt.KeepAlive = 100;

            //var redisConfiguration = new RedisConfiguration()
            //{
            //    AbortOnConnectFail = false,
            //    AllowAdmin = cfgOpt.AllowAdmin,
            //    ConnectTimeout = cfgOpt.ConnectTimeout.AssignIf(x => x < 1000, 5000),
            //    SyncTimeout = cfgOpt.SyncTimeout.AssignIf(x => x < 1000, 5000),
            //    Hosts = cfgOpt.EndPoints.Select(o => new RedisHost()
            //    {
            //        Host = ((System.Net.DnsEndPoint)o).Host,
            //        Port = ((System.Net.DnsEndPoint)o).Port
            //    }).ToArray(),
            //    Database = cfgOpt.DefaultDatabase.Value.AssignIfNotSet(0),
            //    PoolSize = ((IConfigOptions<RedisCache_Option>)m_Option).Value.PoolSize.AssignIfNotSet(10),
            //};

            //Deprecated:
            //var poolMgr = new RedisCacheConnectionPoolManager(redisConfiguration, LogMgr.CreateLogger<RedisClient>());
            //m_Cache = new RedisClient(poolMgr, new NewtonsoftSerializer(), redisConfiguration);

            m_Multiplexer = ConnectionMultiplexer.Connect(connStr);
            m_Cache = m_Multiplexer.GetDatabase(cfgOpt.DefaultDatabase.Value.AssignIfNotSet(0));
        }

        public override async Task<IServiceResponse<T>> GetAsync<T>(string displayKey)
        {
            var result = new ServiceResponse<T>(true);
            var realKey = ConvertToRealKey(displayKey);
            var start = DateTime.UtcNow;

            try
            {
                // command: GET
                //var data = await m_Cache.Db0.GetAsync<T>(realKey);
                var strData = (await m_Cache.StringGetAsync(realKey)).ToString();
                var data = typeof(T) == typeof(string) ? (T)(object)strData : Serializer.Deserialize<T>(strData);
                if (null == data)
                {
                    result.Msg = "Not found. ";
                    CacheMeasurementExtension.WriteCacheMiss(realKey, CacheProviderEnum.Redis, start);
                }
                else
                {
                    result.Content(data);
                    CacheMeasurementExtension.WriteCacheHit(realKey, CacheProviderEnum.Redis, start);
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Redis, start, ex);
            }

            result.SubMsg = Serializer.Serialize(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { SysLoggerKey.MillisecondsDuration, ((DateTime.UtcNow - start).TotalMilliseconds).ToString() }
            });
            return result;
        }

        public override async Task<IDictionary<string, IServiceResponse<T>>> GetAsync<T>(IList<string> displayKeys)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var displayKey in displayKeys)
            {
                var result = await GetAsync<T>(displayKey);
                results.TryAdd(displayKey, result);

                if (false == result.IsSuccess)
                {
                    break;
                }
            };

            return results;
        }

        public override async Task<IServiceResponse<T>> GetOrSetAsync<T>(
            string displayKey,
            Func<T> func,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            // TODO: WATCH displayKey
            var result = await GetAsync<T>(displayKey);
            if (result.Any())
            {
                return result;
            }

            var value = func();
            if (null != value)
            {
                result = await SetAsync(displayKey, value, expiredTime);
                if (result.IsSuccess)
                {
                    result.Content(value);
                }
            }

            return result;
        }

        public override async Task<IServiceResponse<TResult>> GetOrSetAsync<TParam, TResult>(
            string displayKey,
            Func<TParam, TResult> func, TParam param,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            // TODO: WATCH displayKey
            var result = await GetAsync<TResult>(displayKey);
            if (result.Any())
            {
                return result;
            }

            var value = func(param);
            if (null != value)
            {
                result = await SetAsync(displayKey, value, expiredTime);
                if (result.IsSuccess)
                {
                    result.Content(value);
                }
            }

            return result;
        }

        public override async Task<IServiceResponse<T>> SetAsync<T>(
            string displayKey,
            T value,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            var result = new ServiceResponse<T>(true);
            var realKey = ConvertToRealKey(displayKey);
            var start = DateTime.UtcNow;

            try
            {
                expiredTime = expiredTime.ResolveTTL();
                // command: SETEX
                //var isSet = await m_Cache.Db0.AddAsync<T>(realKey, value, DateTime.UtcNow.AddSeconds(expiredTime));
                var data = Serializer.Serialize(value);
                var isSet = await m_Cache.StringSetAsync(realKey, data, TimeSpan.FromSeconds(expiredTime));
                if (isSet)
                {

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result.Error(StatusCodeEnum.Exception, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Redis, start, ex);
            }

            return result;
        }

        public override async Task<IDictionary<string, IServiceResponse<T>>> SetAsync<T>(IDictionary<string, T> items, int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                var res = await SetAsync(item.Key, item.Value, expiredTime);
                if (true != res?.IsSuccess)
                {
                    break;
                }

                results.TryAdd(item.Key, res);
            };

            return results;
        }

        public override async Task<IServiceResponse<bool>> RemoveAsync(string displayKey)
        {
            var result = new ServiceResponse<bool>(true);
            var realKey = ConvertToRealKey(displayKey);
            var start = DateTime.UtcNow;

            try
            {
                // command: DEL
                //var isDel = await m_Cache.Db0.RemoveAsync(realKey);
                var isDel = await m_Cache.KeyDeleteAsync(realKey);
                result.Content(isDel);
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Redis, start, ex);
            }

            return result;
        }

        public override async Task<IDictionary<string, IServiceResponse<bool>>> RemoveAsync(IList<string> displayKeys)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<bool>>(StringComparer.OrdinalIgnoreCase);

            foreach (var displayKey in displayKeys)
            {
                var res = await RemoveAsync(displayKey);
                results.TryAdd(displayKey, res);
                if (true != res?.IsSuccess)
                {
                    break;
                }
            };

            return results;
        }

        public override async Task<IDictionary<string, IServiceResponse<bool>>> RemovePatternAsync(string displayPattern)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<bool>>(StringComparer.OrdinalIgnoreCase);
            var realPattern = displayPattern;
            if (IsAttachPrefixkeyEnabled)
            {
                realPattern = displayPattern.AttachPrefixToKey();
            }

            if (false == realPattern.EndsWith(ConfigConst.CacheKeyScanKeywordSuffix.ToString()))
            {
                realPattern += ConfigConst.CacheKeyScanKeywordSuffix;
            }

            // command: KEYS
            //var realKeys = await m_Cache.Db0.SearchKeysAsync(pattern: realPattern);

            //foreach (var realKey in realKeys)
            //{
            //    var displayKey = ConvertToDispKey(realKey);
            //    var res = await RemoveAsync(displayKey);
            //    if (res.IsSuccess && true == res.Data)
            //    {
            //        results.TryAdd(displayKey, res);
            //    }
            //};

            foreach (var server in m_Multiplexer.GetServers())
            {
                foreach (var key in server.Keys(pattern: realPattern))
                {
                    var displayKey = ConvertToDispKey(key);
                    var res = await RemoveAsync(displayKey);
                    if (res.IsSuccess && true == res.Data)
                    {
                        results.TryAdd(displayKey, res);
                    }
                }
            }

            return results;
        }

        public override async Task<bool> ExistsAsync(string displayKey)
        {
            var realKey = ConvertToRealKey(displayKey);

            try
            {
                // command: EXISTS
                //return await m_Cache.Db0.ExistsAsync(realKey);
                return await m_Cache.KeyExistsAsync(realKey);
            }
            catch { return false; }
        }

        protected ConnectionMultiplexer m_Multiplexer;
        protected IDatabase m_Cache;
        //protected RedisClient m_Cache;
    }
}
