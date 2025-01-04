using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Cache.Measurement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Nwpie.Foundation.Common.Cache
{
    public class DefaultMemoryCache : CacheBase, ILocalCache
    {
        public DefaultMemoryCache(IMemoryCache cache)
            : base(new ConfigOptions<LocalCache_Option>())
        {
            m_Cache = cache;
        }

        #region Get
        //
        // Summary:
        //     Gets value for a given key
        //
        // Parameters:
        //   key:
        //     The key to use as a lookup.
        //
        // Type parameters:
        //   T:
        //     The type T to convert the value to.
        //
        // Returns:
        //     An object implementing the FoundationResponse<T>interface.
        public override async Task<IServiceResponse<T>> GetAsync<T>(string displayKey)
        {
            var result = new ServiceResponse<T>(true);
            var realKey = ConvertToRealKey(displayKey);
            var start = DateTime.UtcNow;

            try
            {
                if (m_Cache.TryGetValue(realKey, out var data))
                {
                    result.Content((T)data);
                    CacheMeasurementExtension.WriteCacheHit(realKey, CacheProviderEnum.Local, start);
                }
                else
                {
                    result.Msg = "Not found. ";
                    CacheMeasurementExtension.WriteCacheMiss(realKey, CacheProviderEnum.Local, start);
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Error, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Local, start, ex);
            }

            result.SubMsg = Serializer.Serialize(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { SysLoggerKey.MillisecondsDuration, ((DateTime.UtcNow - start).TotalMilliseconds).ToString() }
            });
            await Task.CompletedTask;
            return result;
        }

        //
        // Summary:
        //     Gets a range of values for a given set of keys
        //
        // Parameters:
        //   keys:
        //     The keys to get
        //
        // Type parameters:
        //   T:
        //     The System.Type of the values to be returned
        //
        // Returns:
        //     A System.Collections.Generic.Dictionary<TKey,TValue> of the keys sent and
        //     the FoundationResponse<T> result.
        //
        public override async Task<IDictionary<string, IServiceResponse<T>>> GetAsync<T>(IList<string> displayKeys)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var displayKey in displayKeys)
            {
                var result = await GetAsync<T>(displayKey);
                if (false == result.IsSuccess)
                {
                    break;
                }

                results.TryAdd(displayKey, result);
            };

            return results;
        }
        #endregion

        #region Set
        //
        // Summary:
        //     Inserts or replaces an existing value into local cache.
        //
        // Parameters:
        //   key:
        //     The unique key for indexing.
        //
        //   value:
        //     The value for the key.
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is ConfigConst.DefaultCacheSecs seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        // Returns:
        //     An object implementing the FoundationResponse<T>interface.
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
                var cachePolicy = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.UtcNow.AddSeconds(expiredTime),
                    Priority = CacheItemPriority.Normal
                };

                m_Cache.Set(realKey, value, cachePolicy);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result.Error(StatusCodeEnum.Error, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Local, start, ex);
            }

            await Task.CompletedTask;
            return result;
        }

        //
        // Summary:
        //     Inserts or replaces a range of items into local cache.
        //
        // Parameters:
        //   items:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of items to be stored
        //     in local cache.
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is ConfigConst.DefaultCacheSecs seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        //
        // Returns:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of FoundationResponse<T>
        //     which for which each is the result of the individual operation.
        //
        public override async Task<IDictionary<string, IServiceResponse<T>>> SetAsync<T>(
            IDictionary<string, T> items,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<T>>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                var res = await SetAsync(item.Key, item.Value, expiredTime);
                if (false == res.IsSuccess)
                {
                    break;
                }

                results.TryAdd(item.Key, res);
            };

            return results;
        }
        #endregion

        #region GetOrSet
        //
        // Summary:
        //     Caching the result of a method.
        //
        // Parameters:
        //   func:
        //     delegate function
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is ConfigConst.DefaultCacheSecs seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        //
        // Returns:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of FoundationResponse<T>
        //     which for which each is the result of the individual operation.
        public override async Task<IServiceResponse<T>> GetOrSetAsync<T>(
            string displayKey,
            Func<T> func,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
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

        //
        // Summary:
        //     Caching the result of a method.
        //
        // Parameters:
        //   func:
        //     delegate function
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is ConfigConst.DefaultCacheSecs seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        //
        // Returns:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of FoundationResponse<T>
        //     which for which each is the result of the individual operation.
        public override async Task<IServiceResponse<TResult>> GetOrSetAsync<TParam, TResult>(
            string key,
            Func<TParam, TResult> func,
            TParam param, int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            var result = await GetAsync<TResult>(key);
            if (result.Any())
            {
                return result;
            }

            var value = func(param);
            if (null != value)
            {
                result = await SetAsync(key, value, expiredTime);
                if (result.IsSuccess)
                {
                    result.Content(value);
                }
            }

            return result;
        }
        #endregion

        #region Remove
        //
        // Summary:
        //     Removes a value for a given key from the local cache.
        //
        // Parameters:
        //   key:
        //     The key to remove from the local cache
        //
        // Returns:
        //     An object implementing the FoundationResponse<T>interface.
        public override async Task<IServiceResponse<bool>> RemoveAsync(string displayKey)
        {
            var result = new ServiceResponse<bool>(true);
            var realKey = ConvertToRealKey(displayKey);
            var start = DateTime.UtcNow;

            try
            {
                m_Cache.Remove(realKey);
                result.Content(true);
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Error, ex);
                CacheMeasurementExtension.WriteCacheException(realKey, CacheProviderEnum.Local, start, ex);
            }

            await Task.CompletedTask;
            return result;
        }

        //
        // Summary:
        //     Removes a range of values for a given set of keys
        //
        // Parameters:
        //   keys:
        //     The keys to remove
        //
        // Returns:
        //     A System.Collections.Generic.Dictionary<TKey,TValue> of the keys sent and
        //     the FoundationResponse result.
        public override async Task<IDictionary<string, IServiceResponse<bool>>> RemoveAsync(IList<string> displayKeys)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<bool>>(StringComparer.OrdinalIgnoreCase);
            foreach (var displayKey in displayKeys)
            {
                var res = await RemoveAsync(displayKey);
                if (false == res.IsSuccess)
                {
                    break;
                }

                results.TryAdd(displayKey, res);
            };

            return results;
        }

        public override async Task<IDictionary<string, IServiceResponse<bool>>> RemovePatternAsync(string displayPattern)
        {
            var results = new ConcurrentDictionary<string, IServiceResponse<bool>>(StringComparer.OrdinalIgnoreCase);
            var start = DateTime.UtcNow;

            var realPattern = ConvertToRealKey(displayPattern);
            if (realPattern.EndsWith(ConfigConst.CacheKeyScanKeywordSuffix.ToString()))
            {
                realPattern = realPattern.TrimEnd(ConfigConst.CacheKeyScanKeywordSuffix);
            }

            //IDictionary cacheMap;
            IEnumerable<object> cacheMap;
            try
            {
                // TODO: Don't HACK it
                //var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
                //cacheMap = field.GetValue(m_Cache) as IDictionary;
                var coherentStateField = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
                var coherentState = coherentStateField.GetValue(m_Cache as MemoryCache);
                var getAllKeysMethod = coherentState.GetType().GetMethod("GetAllKeys", BindingFlags.Public | BindingFlags.Instance);
                var keys = getAllKeysMethod.Invoke(coherentState, null) as IEnumerable<object>;
                cacheMap = keys;
            }
            catch (Exception ex)
            {
                results.TryAdd(displayPattern, new ServiceResponse<bool>()
                    .Error(StatusCodeEnum.Error, ex));
                CacheMeasurementExtension.WriteCacheException(realPattern, CacheProviderEnum.Local, start, ex);
                return results;
            }

            if (cacheMap?.Count() > 0)
            {
                foreach (var item in cacheMap)
                {
                    var displayKey = ConvertToDispKey(item.ToString());
                    if (Regex.IsMatch(item.ToString(), realPattern))
                    {
                        var res = await RemoveAsync(displayKey);
                        if (res.IsSuccess && res.Data)
                        {
                            results.TryAdd(displayKey, res);
                        }
                    }
                };
            }

            return results;
        }
        #endregion

        //
        // Summary:
        //     Checks for the existance of a given key.
        //
        // Parameters:
        //   key:
        //     The key to check.
        //
        // Returns:
        //     True if the key exists.
        public override async Task<bool> ExistsAsync(string displayKey)
        {
            var isExist = false;
            try
            {
                var realKey = ConvertToRealKey(displayKey);
                isExist = m_Cache.TryGetValue(realKey, out _);
            }
            catch { }

            await Task.CompletedTask;
            return isExist;
        }

        protected readonly IMemoryCache m_Cache;
    }
}
