using System;
using System.Linq;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Configuration.SDK.Extensions
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddConfigServer<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IConfigClient
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new DefaultRemoteConfigClient(
                    p.GetService<ISerializer>()
                ), lifetime)
            );

            return services;
        }

        public static IConfigurationBuilder SdkConfiguration(this IConfigurationBuilder config)
        {
            JsonConvert.DefaultSettings = () => new DefaultSerializer().Settings;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddProvider(ServiceContext.Config)
            ;

            return config;
        }

        public static IConfigurationBuilder AddProvider(this IConfigurationBuilder builder, ServiceConfiguration config)
        {
            // CAUTION: Unstable ordering, ex: appsettings.*.json
            if (config?.StartupJsonFileList?.Count() > 0)
            {
                //var ordered = config.StartupJsonFileList.OrderBy(o => o.Value);
                foreach (var item in config.StartupJsonFileList)
                {
                    builder.AddJsonFile(item.Key, optional: item.Value, reloadOnChange: true);
                }
            }

            if (config?.RemoteConfigKeys?.Count() > 0)
            {
                var distinct = config.RemoteConfigKeys.Distinct();
                foreach (var item in distinct)
                {
                    builder.FromConfigServer<string>(item);
                }
            }

            return builder;
        }
    }
}
