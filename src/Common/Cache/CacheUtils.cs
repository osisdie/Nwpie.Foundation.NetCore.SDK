using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common.Utilities;

namespace Nwpie.Foundation.Common.Cache
{
    public static class CacheUtils
    {
        public static bool IsHealthy(ICache cache)
        {
            if (null == cache)
            {
                return false;
            }

            var cacheVal = IdentifierUtils.NewId();
            var cacheKey = $"{ServiceContext.ApiName}-{cache.GetType().Name}-healthcheck-{cacheVal}";

            // Test roundtrip
            var setResult = cache.SetAsync(cacheKey, cacheVal, 10).ConfigureAwait(false).GetAwaiter().GetResult();
            if (true == setResult?.IsSuccess)
            {
                var getResult = cache.GetAsync<string>(cacheKey).ConfigureAwait(false).GetAwaiter().GetResult();
                if (getResult.Any())
                {
                    return true;
                }
            }

            return false;
        }

        public static int ResolveTTL(this int ttl) =>
            ttl <= 0
            ? ConfigConst.DefaultCacheSecs
            : ttl;

        public static string CacheKeyWithFuncAndParam(string paramString, [CallerFilePath]string callerFilePath = null, [CallerMemberName] string caller = "")
        {
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
            var sb = new StringBuilder(Utility.GetCallerAsmVersion());
            sb.Append(__);
            sb.Append(fileName);
            sb.Append(__);
            sb.Append(caller);
            sb.Append(__);
            sb.Append(paramString);
            return sb.ToString();
        }

        public static string AttachPrefixToKey(this string key) =>
            string.Concat(ServiceContext.ApiName, __, key);

        public static IList<string> AttachPrefixToKey(IList<string> keys)
        {
            var result = new List<string>();
            foreach (var key in keys)
            {
                result.Add(key.AttachPrefixToKey());
            }

            return result;
        }

        public static IDictionary<string, T> AttachPrefixToKey<T>(IDictionary<string, T> items)
        {
            var dic = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                dic.Add(item.Key.AttachPrefixToKey(), item.Value);
            }

            return dic;
        }

        public static string DetachPrefixFromKey(this string key) =>
            key.Remove(0, ServiceContext.ApiName.Length + __.Length);

        public static IList<string> DetachPrefixFromKey(IList<string> keys)
        {
            var result = new List<string>();
            foreach (var key in keys)
            {
                result.Add(key.DetachPrefixFromKey());
            }

            return result;
        }

        public const string __ = CommonConst.Separator;
    }
}
