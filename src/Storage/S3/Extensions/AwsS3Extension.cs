using System;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Storage.S3.Extensions
{
    public static class AwsS3Extension
    {
        public static IServiceCollection AddDefaultS3Option<TModel>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TModel : AwsS3_Option, new()
        {
            var proto = typeof(ConfigOptions<>);
            Type[] typeArgs = { typeof(TModel) };
            var make = proto.MakeGenericType(typeArgs);

            services.AddSingleton(typeof(IConfigOptions<TModel>), p =>
                Activator.CreateInstance(make,
                    configKey.ConfigServerValue<TModel>()
                )
            );

            return services;
        }

        public static IServiceCollection AddDefaultS3Service<TService>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IS3StorageClient
        {
            // TODO: AwsS3_Option => TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new S3StorageClient(
                    new ConfigOptions<AwsS3_Option>(
                        configKey.ConfigServerValue<AwsS3_Option>()
                    ), p.GetService<ICache>()
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddS3Factory<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IAwsS3Factory
        {
            services.Add(new ServiceDescriptor(typeof(TService),
                typeof(AwsS3Factory), lifetime));
            return services;
        }

    }
}
