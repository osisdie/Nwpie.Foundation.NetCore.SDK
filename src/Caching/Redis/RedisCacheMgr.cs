using System;
using System.Collections.Concurrent;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Patterns;

namespace Nwpie.Foundation.Caching.Redis
{
    public class RedisCacheMgr : SingleCObject<RedisCacheMgr>
    {
        protected override void InitialInConstructor() { }

        public IRedisCache GetCache(IConfigOptions<RedisCache_Option> option)
        {
            return m_CacheMap
                .GetOrAdd(option.ToString(), new RedisCache(option));
        }

        public override void Dispose()
        {
            m_CacheMap?.Clear();
        }

        private readonly ConcurrentDictionary<string, IRedisCache> m_CacheMap = new ConcurrentDictionary<string, IRedisCache>(StringComparer.OrdinalIgnoreCase);
    }
}
