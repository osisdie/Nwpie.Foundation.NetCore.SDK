using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nwpie.Foundation.Hosting.ServiceStack.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDefaultSerializer<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TService : ISerializer
        {
            services.Add(new ServiceDescriptor(typeof(IHttpSerializer),
                typeof(DefaultHttpSerializer), lifetime));

            services.Add(new ServiceDescriptor(typeof(IJsonConfigSerializer),
                typeof(DefaultJsonConfigSerializer), lifetime));

            services.Add(new ServiceDescriptor(typeof(TService),
                typeof(DefaultSerializer), lifetime));

            return services;
        }

        public static IServiceCollection RegisterAssemblyTypes<T>(this IServiceCollection services,
            ServiceLifetime lifetime,
            params Assembly[] assemblies) =>
            services.RegisterAssemblyTypes<T>(lifetime: lifetime,
                includeBaseInterface: false,
                predicates: null,
                assembly: null,
                assemblies: assemblies
            );

        public static IServiceCollection RegisterAssemblyTypes<T>(this IServiceCollection services,
            ServiceLifetime lifetime,
            bool includeBaseInterface,
            List<Func<TypeInfo, bool>> predicates,
            Assembly assembly,
            params Assembly[] assemblies)
        {
            var scanAssemblies = new List<Assembly>();
            if (null != assembly)
            {
                scanAssemblies.Add(assembly);
            }

            if (assemblies?.Count() > 0)
            {
                scanAssemblies.AddRange(assemblies);
            }

            scanAssemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => false == scanAssemblies.Any(a => a.FullName == t.FullName))
                .Distinct()
                .ToList()
                .ForEach(x => scanAssemblies.Add(AppDomain.CurrentDomain.Load(x)));

            var interfaces = scanAssemblies.SelectMany(o => o.DefinedTypes
                .Where(x => x.IsInterface)
                //.Where(x => x.GetInterfaces().Contains(typeof(T)))
                .Where(x => typeof(T).IsAssignableFrom(x))
            );

            if (false == includeBaseInterface)
            {
                interfaces = interfaces.Where(x => x != typeof(T));
            }

            foreach (var @interface in interfaces)
            {
                var types = scanAssemblies.SelectMany(o => o.DefinedTypes
                    .Where(x => x.IsClass)
                    //.Where(x => x.GetInterfaces().Contains(typeof(T)))
                    .Where(x => @interface.IsAssignableFrom(x))
                );

                if (predicates?.Count() > 0)
                {
                    foreach (var predict in predicates)
                    {
                        types = types.Where(predict);
                    }
                }

                foreach (var type in types)
                {
                    services.TryAdd(new ServiceDescriptor(@interface,
                        type,
                        lifetime)
                    );
                }
            }

            return services;
        }

        public static IServiceCollection RegisterAllTypes<T>(this IServiceCollection services,
            ServiceLifetime lifetime,
            List<Func<TypeInfo, bool>> predicates,
            Assembly assembly,
            params Assembly[] assemblies)
        {
            var scanAssemblies = new List<Assembly>();
            if (null != assembly)
            {
                scanAssemblies.Add(assembly);
            }

            if (assemblies?.Count() > 0)
            {
                scanAssemblies.AddRange(assemblies);
            }

            scanAssemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => false == scanAssemblies.Any(a => a.FullName == t.FullName))
                .Distinct()
                .ToList()
                .ForEach(x => scanAssemblies.Add(AppDomain.CurrentDomain.Load(x)));

            var types = scanAssemblies.SelectMany(o => o.DefinedTypes
                .Where(x => typeof(T).IsAssignableFrom(x))
            //.Where(x => x.GetInterfaces().Contains(typeof(T)))
            );

            if (predicates?.Count() > 0)
            {
                foreach (var predict in predicates)
                {
                    types = types.Where(predict);
                }
            }

            foreach (var type in types)
            {
                services.TryAdd(new ServiceDescriptor(typeof(T),
                    type,
                    lifetime)
                );
            }

            return services;
        }
    }
}
