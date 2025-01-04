using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.HostTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nwpie.HostTest.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDerivedRepository(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var scanAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var interfaces = scanAssemblies.SelectMany(o => o.DefinedTypes
                .Where(x => x.IsInterface)
                .Where(x => x != typeof(IRepository)) // exclude super interface
                .Where(x => typeof(IRepository).IsAssignableFrom(x))
            );

            foreach (var @interface in interfaces)
            {
                var types = scanAssemblies.SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass)
                    .Where(x => @interface.IsAssignableFrom(x))
                );

                foreach (var type in types)
                {
                    services.TryAdd(new ServiceDescriptor(@interface, type, lifetime));
                }
            }

            return services;
        }
    }
}
