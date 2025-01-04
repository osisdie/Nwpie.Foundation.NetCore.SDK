using System;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Caching.Redis.Extensions
{
    public static class RedisExtension
    {
        static RedisExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(RedisExtension));
        }

        public static IServiceCollection AddDefaultRedisConnectionString<TModel>(this IServiceCollection services, string conn, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TModel : RedisCache_Option, new()
        {
            services.Add(new ServiceDescriptor(typeof(IConfigOptions<TModel>), p =>
                new ConfigOptions<RedisCache_Option>(
                    new RedisCache_Option
                    {
                        // Ex: ConnectionString = "localhost:6379,connectTimeout=5000,syncTimeout=60000,defaultDatabase=0"
                        ConnectionString = conn
                    }
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddDefaultRedisOption<TModel>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TModel : RedisCache_Option, new()
        {
            var proto = typeof(ConfigOptions<>);
            Type[] typeArgs = { typeof(TModel) };
            var make = proto.MakeGenericType(typeArgs);

            services.Add(new ServiceDescriptor(typeof(IConfigOptions<TModel>), p =>
                Activator.CreateInstance(make,
                    configKey.ConfigServerValue<TModel>()
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddRedisCache<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IRedisCache
        {
            // TODO: RedisCache_Option -> TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new RedisCache(
                    p.GetService<IConfigOptions<RedisCache_Option>>()
                ), lifetime));

            return services;
        }

        public static ICache GetRedisCacheOrAlternative(
            string serviceName,
            bool isHealthCheck = false)
        {
            var cache = ComponentMgr.Instance.TryResolve<IRedisCache>();
            if (null != cache)
            {
                if (false == isHealthCheck)
                {
                    return cache;
                }

                try
                {
                    // Test roundtrip
                    var cacheVal = IdentifierUtils.NewId();
                    var cacheKey = $"{serviceName}-cache-{cache.GetType().Name}-healthcheck-{cacheVal}";
                    var setResult = cache.SetAsync(cacheKey, cacheVal, 10).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (true == setResult?.IsSuccess)
                    {
                        var getResult = cache.GetAsync<string>(cacheKey).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (getResult.Any())
                        {
                            return cache;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex.ToString());
                }
            }

            return ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
        }

        private static readonly ILogger Logger;
    }
}
