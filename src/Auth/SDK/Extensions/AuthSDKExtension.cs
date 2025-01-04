using System;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Common.Config.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Auth.SDK.Extensions
{
    public static class AuthSDKExtension
    {
        public static IServiceCollection AddDefaultAuthOption<TModel>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TModel : Auth_Option, new()
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

        public static IServiceCollection AddDefaultAuthService<TService, TProvider>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ITokenService
            where TProvider : ITokenService
        {
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                p.GetService(typeof(TProvider)), lifetime));

            return services;
        }

        public static IServiceCollection AddApiKeyAuthService<TService, TModel>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IApiKeyAuthService
            where TModel : class, ITokenDataModel, new()
        {
            var proto = typeof(DefaultApiKeyAuthService<>);
            Type[] typeArgs = { typeof(TModel) };
            var make = proto.MakeGenericType(typeArgs);

            // TODO: Auth_Option -> TAuthModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                Activator.CreateInstance(make,
                    p.GetService<IConfigOptions<Auth_Option>>(),
                    p.GetService<ISerializer>(),
                    p.GetService<ICache>()
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddJwtAuthService<TService, TModel>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IJwtAuthService
            where TModel : class, ITokenDataModel, new()
        {
            var proto = typeof(DefaultJwtAuthService<>);
            Type[] typeArgs = { typeof(TModel) };
            var make = proto.MakeGenericType(typeArgs);

            // TODO: Auth_Option -> TAuthModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                Activator.CreateInstance(make,
                    p.GetService<IConfigOptions<Auth_Option>>(),
                    p.GetService<ISerializer>(),
                    p.GetService<ICache>()
                ), lifetime));

            return services;
        }

        public static IServiceCollection AddAesAuthService<TService, TModel>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : IAesAuthService
            where TModel : class, ITokenDataModel, new()
        {
            var proto = typeof(DefaultAesAuthService<>);
            Type[] typeArgs = { typeof(TModel) };
            var make = proto.MakeGenericType(typeArgs);

            // TODO: Auth_Option -> TAuthModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                Activator.CreateInstance(make,
                    p.GetService<IConfigOptions<Auth_Option>>(),
                    p.GetService<ISerializer>(),
                    p.GetService<ICache>()
                ), lifetime));

            return services;
        }
    }
}
