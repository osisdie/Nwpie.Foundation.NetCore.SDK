using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Common.Storage.Extensions
{
    public static class LocalStorageExtension
    {
        public static IServiceCollection AddLocalStorage<TService>(this IServiceCollection services, string basePath, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ILocalStorageClient
        {
            // TODO: LocalStorage_Option => TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new LocalStorageClient(
                    new ConfigOptions<LocalStorage_Option>(
                        new LocalStorage_Option()
                        {
                            BasePath = basePath
                        }
                    ), p.GetService<ICache>()
                ), lifetime));

            return services;
        }
    }
}
