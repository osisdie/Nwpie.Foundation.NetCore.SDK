using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Common.Extras
{
    public class ComponentMgr : SingleCObject<ComponentMgr>
    {
        protected override void InitialInConstructor()
        {
            // Default
            _ = m_SingletonMap.GetOrAdd(typeof(ILocalCache), new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                TrackStatistics = true
            })));
            _ = m_SingletonMap.GetOrAdd(typeof(ISerializer), new DefaultSerializer());
        }

        public void ChangeDefaultSerializer(ISerializer serializer)
        {
            _ = m_SingletonMap.AddOrUpdate(
                typeof(ISerializer),
                serializer,
                (k, v) => serializer
            );
        }

        public void ChangeDefaultLocalCache(ILocalCache cache)
        {
            _ = m_SingletonMap.AddOrUpdate(
                typeof(ILocalCache),
                cache,
                (k, v) => cache
            );
        }

        public ISerializer SerializerFromDI { get => TryResolve<ISerializer>() ?? GetDefaultSerializer(); }
        public ISerializer GetDefaultSerializer(bool isUseDI = false)
        {
            return (ISerializer)m_SingletonMap.GetOrAdd(typeof(ISerializer), (T) =>
            {
                if (isUseDI)
                {
                    var serializer = TryResolve<ISerializer>();
                    if (null != serializer)
                    {
                        return serializer;
                    }
                }

                return new DefaultSerializer();
            });
        }

        public INotificationHttpClient GetDefaultNotificationHttpClient() => TryResolve<INotificationHttpClient>();
        public INotificationSQSClient GetDefaultNotificationSQSClient() => TryResolve<INotificationSQSClient>();
        public IStorage GetDefaultStorageClient() => TryResolve<IStorage>();
        public ICache CacheFromDI { get => TryResolve<ICache>() ?? GetDefaultLocalCache(); }

        public ICache GetDefaultCache(bool isFailOverToLocalCache = true, ICache defaultCache = null)
        {
            var cache = TryResolve<ICache>();
            if (null == cache)
            {
                cache = isFailOverToLocalCache
                    ? GetDefaultLocalCache()
                    : defaultCache;
            }

            return cache;
        }

        public ILocalCache LocalCacheFromDI { get => TryResolve<ILocalCache>() ?? GetDefaultLocalCache(); }
        // Also can use .TryResolve<ILocalCache> for Runtime DI
        public ILocalCache GetDefaultLocalCache(bool isUseDI = false)
        {
            return (ILocalCache)m_SingletonMap.GetOrAdd(typeof(ILocalCache), (T) =>
            {
                if (isUseDI)
                {
                    var cache = TryResolve<ILocalCache>();
                    if (cache != null)
                    {
                        return cache;
                    }
                }

                return new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
                {
                    TrackStatistics = true
                }));
            });
        }

        public IRedisCache GetDefaultRedisCache() => TryResolve<IRedisCache>();

        public ICache GetCache<T>(bool isHealthCheck = false, bool isFailOverToLocalCache = false, ICache defaultCache = null)
            where T : class, ICache
        {
            if (!(TryResolve<T>() is ICache cache))
            {
                cache = isFailOverToLocalCache
                    ? GetDefaultLocalCache()
                    : defaultCache;
            }

            if (isHealthCheck)
            {
                cache = ReplaceUnHealthyCache<T>(defaultCache);
            }

            return cache;
        }

        public ICache CacheFromConfig { get => GetDefaultCacheFromConfig(); }

        public ICache GetDefaultCacheFromConfig(
            string defaultCacheProvider = "LocalCache",
            bool isHealthCheck = false)
        {
            var provider = ServiceContext.Config.CACHE?.DefaultProvider
                ?? defaultCacheProvider;

            if (true == provider?.StartsWith(CacheProviderEnum.Local.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return GetDefaultLocalCache();
            }

            if (true == provider?.StartsWith(CacheProviderEnum.Redis.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var cache = GetDefaultRedisCache();
                if (null != cache)
                {
                    if (isHealthCheck)
                    {
                        return ReplaceUnHealthyCache<IRedisCache>();
                    }

                    return cache;
                }
            }

            return GetDefaultLocalCache();
        }

        public ICache ReplaceUnHealthyCache<T>(ICache defaultCache = null)
            where T : class, ICache
        {
            var cache = TryResolve<T>();
            if (null == cache)
            {
                return defaultCache ?? GetDefaultLocalCache();
            }

            try
            {
                var isHealthy = CacheUtils.IsHealthy(cache);
                if (isHealthy)
                {
                    return cache;
                }

                Logger.LogWarning($"Cache (={typeof(T).Name}) is NOT healthy. ");

            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Cache (={typeof(T).Name}) is NOT healthy (ex={ex}). ");
            }

            return defaultCache ?? GetDefaultLocalCache();
        }

        public T GetCache<T>(IConfigOptions option)
            where T : ICache
        {
            if (null == option)
            {
                throw new ArgumentNullException(nameof(IConfigOptions));
            }

            return Resolve<T>(new TypedParameter(option.GetType(), option));
        }

        public T Resolve<T>(params Parameter[] parameters) => Resolve<T>(parameters?.ToList());
        public T Resolve<T>(IEnumerable<Parameter> parameters)
        {
            if (parameters?.Count() > 0)
            {
                return DIContainer.Resolve<T>(parameters);
            }

            return DIContainer.Resolve<T>();
        }

        public T TryResolve<T>() where T : class
        {
            try
            {
                if (DIContainer.TryResolve<T>(out var component))
                {
                    return component;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return default(T);
        }

        public bool TryResolve<T>(out T component) where T : class
        {
            return DIContainer.TryResolve<T>(out component);
        }

        //public void AfterBuild(Autofac.IContainer container)
        public void AfterBuild(Autofac.ILifetimeScope container)
        {
            DIContainer = container;
            IsReady = true;
            Logger.LogTrace($"Finished IOC.build() by {Utility.GetCallerFullName(3)}. Ready to Resolve<T>(). ");
        }

        public void ManualBuild(Autofac.ContainerBuilder builder)
        {
            if (null == DIContainer)
            {
                try
                {
                    DIContainer = builder.Build();
                    IsReady = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    throw;
                }
            }
        }

        public override void Dispose()
        {
            DIContainer?.Dispose();
        }

        public Autofac.ILifetimeScope DIContainer { get; set; }
        //public Autofac.IContainer DIContainer { get; set; }

        private Autofac.ContainerBuilder m_DIBuilder;
        public Autofac.ContainerBuilder DIBuilder
        {
            get
            {
                return m_DIBuilder;
            }
            set
            {
                m_DIBuilder = value;
                m_DIBuilder.RegisterBuildCallback(AfterBuild);
                Logger.LogTrace($"Start building... ");
            }
        }

        public Autofac.ILifetimeScope LifetimeScope { get; set; }
        public bool IsReady { get; private set; }

        protected readonly ConcurrentDictionary<Type, object> m_SingletonMap = new ConcurrentDictionary<Type, object>();
        private readonly object m_Lock = new object();
    }
}
