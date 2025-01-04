using System.Linq;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Common.Cache.Extensions
{
    public static class LocalCacheExtension
    {
        public static IServiceCollection AddLocalCache<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ILocalCache
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new DefaultMemoryCache(
                   new MemoryCache(new MemoryCacheOptions
                   {
                       TrackStatistics = true
                   })
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddAsDefaultICache<TProvider>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TProvider : ICache
        {
            if (services.Any(sd => sd.ServiceType == typeof(ICache)))
            {
                return services;
            }

            services.Add(new ServiceDescriptor(typeof(ICache), p =>
                p.GetService(typeof(TProvider)), lifetime));

            return services;
        }

        public static IServiceCollection AddAsDefaultICache<TService>(this IServiceCollection services, TService provider, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ICache
        {
            if (services.Any(sd => sd.ServiceType == typeof(ICache)))
            {
                return services;
            }

            services.Add(new ServiceDescriptor(typeof(ICache), p =>
                provider, lifetime));

            return services;
        }
    }
}
