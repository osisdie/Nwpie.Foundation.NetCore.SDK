using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Nwpie.Foundation.Caching.LocalCache
{
    public class LocalCache : CacheBase, ILocalCache
    {
        public LocalCache(IConfigOptions<LocalCache_Option> option)
            : base(option)
        {
        }

        protected override void Initialization()
        {
            m_Cache = new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                TrackStatistics = true
            }));
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
        public override Task<IServiceResponse<T>> GetAsync<T>(string displayKey) =>
            m_Cache.GetAsync<T>(displayKey);

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
        public override Task<IDictionary<string, IServiceResponse<T>>> GetAsync<T>(IList<string> displayKeys) =>
            m_Cache.GetAsync<T>(displayKeys);

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
        public override async Task<IServiceResponse<T>> SetAsync<T>(string displayKey, T value, int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            return await m_Cache.SetAsync<T>(displayKey, value, expiredTime);
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
        public override async Task<IDictionary<string, IServiceResponse<T>>> SetAsync<T>(IDictionary<string, T> items, int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            return await m_Cache.SetAsync<T>(items, expiredTime);
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
            return await m_Cache.GetOrSetAsync<T>(displayKey, func, expiredTime);
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
            string displayKey,
            Func<TParam, TResult> func,
            TParam param,
            int expiredTime = ConfigConst.DefaultCacheSecs)
        {
            return await m_Cache.GetOrSetAsync<TParam, TResult>(displayKey, func, param, expiredTime);
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
            return await m_Cache.RemoveAsync(displayKey);
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
            return await m_Cache.RemoveAsync(displayKeys);
        }

        public override async Task<IDictionary<string, IServiceResponse<bool>>> RemovePatternAsync(string displayPattern)
        {
            return await m_Cache.RemovePatternAsync(displayPattern);
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
            return await m_Cache.ExistsAsync(displayKey);
        }

        protected ICache m_Cache;
    }
}
