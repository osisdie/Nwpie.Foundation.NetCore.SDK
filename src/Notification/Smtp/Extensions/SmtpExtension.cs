using System;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Notification.Smtp.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.Notification.Smtp.Extensions
{
    public static class SmtpExtension
    {
        public static IServiceCollection AddDefaultSmtpOption<TModel>(this IServiceCollection services, string configKey, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TModel : Smtp_Option, new()
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

        public static IServiceCollection AddSmtpService<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ISmtpMailService
        {
            // TODO: Smtp_Option -> TModel
            services.Add(new ServiceDescriptor(typeof(TService), p =>
                new SmtpMailService(
                    p.GetService<IConfigOptions<Smtp_Option>>()
                ), lifetime));

            return services;
        }
    }
}
