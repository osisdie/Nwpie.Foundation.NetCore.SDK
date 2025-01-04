using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Location.SDK.Extensions
{
    public static class LocationExtension
    {
        public static IServiceCollection AddLocationService<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ILocationClient
        {
            services.Add(new ServiceDescriptor(typeof(TService),
                typeof(LocationClient), lifetime)
            );
            return services;
        }
    }
}
