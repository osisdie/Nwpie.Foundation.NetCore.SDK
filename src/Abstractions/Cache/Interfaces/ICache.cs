using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Abstractions.Cache.Interfaces
{
    public interface ICache : ICObject
    {
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
        //     An object implementing the FoundationResponse<T> interface.
        Task<IServiceResponse<T>> GetAsync<T>(string displayKey);

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
        Task<IDictionary<string, IServiceResponse<T>>> GetAsync<T>(IList<string> displayKeys);

        //
        // Summary:
        //     Inserts or replaces an existing value into cache.
        //
        // Parameters:
        //   key:
        //     The unique key for indexing.
        //
        //   value:
        //     The value for the key.
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is 3600 seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        // Returns:
        //     An object implementing the FoundationResponse<T>interface.
        Task<IServiceResponse<T>> SetAsync<T>(
            string displayKey,
            T value,
            int expiredTime = ConfigConst.DefaultCacheSecs);

        //
        // Summary:
        //     Inserts or replaces a range of items into cache.
        //
        // Parameters:
        //   items:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of items to be stored
        //     in cache.
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is 3600 seconds.
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
        Task<IDictionary<string, IServiceResponse<T>>> SetAsync<T>(
            IDictionary<string, T> items,
            int expiredTime = ConfigConst.DefaultCacheSecs);

        //
        // Summary:
        //     Caching the result of a method.
        //
        // Parameters:
        //   func:
        //     delegate function
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is 3600 seconds.
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
        Task<IServiceResponse<T>> GetOrSetAsync<T>(
            string displayKey,
            Func<T> func,
            int expiredTime = ConfigConst.DefaultCacheSecs)
            where T : class;

        //
        // Summary:
        //     Caching the result of a method.
        //
        // Parameters:
        //   func:
        //     delegate function
        //
        //   expiredTime:(unit / seconds)
        //     The time-to-live (ttl) for the counter. default value is 3600 seconds.
        //
        // Type parameters:
        //   T:
        //     The Type of the value to be inserted.
        //
        //
        // Returns:
        //     A System.Collections.Generic.IDictionary<TKey,TValue> of FoundationResponse<T>
        //     which for which each is the result of the individual operation.
        Task<IServiceResponse<TResult>> GetOrSetAsync<TParam, TResult>(
            string displayKey,
            Func<TParam, TResult> func,
            TParam param,
            int expiredTime = ConfigConst.DefaultCacheSecs)
            where TResult : class;

        //
        // Summary:
        //     Removes a value for a given key from the cache.
        //
        // Parameters:
        //   key:
        //     The key to remove from the cache
        //
        // Returns:
        //     An object implementing the FoundationResponse<T>interface.
        Task<IServiceResponse<bool>> RemoveAsync(string displayKey);

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
        Task<IDictionary<string, IServiceResponse<bool>>> RemoveAsync(IList<string> displayKeys);

        // Summary:
        //     Removes a range of values for a given prefix key pattern
        //
        // Parameters:
        //   keys:
        //     The prefix key to remove
        //
        // Returns:
        //     A System.Collections.Generic.Dictionary<TKey,TValue> of the keys sent and
        //     the FoundationResponse result.
        Task<IDictionary<string, IServiceResponse<bool>>> RemovePatternAsync(string displayPattern);

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
        Task<bool> ExistsAsync(string displayKey);

        string ConvertToRealKey(string displayKey);
        string ConvertToDispKey(string realKey);
    }
}
