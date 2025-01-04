using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Nwpie.Foundation.DataAccess.Mapper.Extensions
{
    public static class MapperExtension
    {
        public static IServiceCollection AddMappers<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where T : AutoMapper.IMapper
        {
            var assemblies = AppDomain.CurrentDomain
               .GetAssemblies()
               .ToList();
            assemblies.SelectMany(x => x.GetReferencedAssemblies())
                //.Where(t =>
                //    t.Name.StartsWith($"{CommonConst.SdkPrefix}.", StringComparison.OrdinalIgnoreCase))
                .Where(t => false == assemblies
                    .Any(a =>
                        a.FullName == t.FullName &&
                        a.GetName().Name == nameof(AutoMapper)
                    )
                )
                .Distinct()
                .ToList()
                .ForEach(x =>
                    assemblies.Add(AppDomain.CurrentDomain.Load(x))
                );
            var asm = assemblies.ToArray();
            var allTypes = assemblies
               .Where(a =>
                    false == a.IsDynamic &&
                    a.GetName().Name != nameof(AutoMapper)
                )
               .Distinct()
               .SelectMany(a => a.DefinedTypes)
               .ToArray();

            var openTypes = new[]
            {
                typeof(AutoMapper.IValueResolver<,,>),
                typeof(AutoMapper.IMemberValueResolver<,,,>),
                typeof(AutoMapper.ITypeConverter<,>),
                typeof(AutoMapper.IValueConverter<,>),
                typeof(AutoMapper.IMappingAction<,>)
            };

            foreach (var type in openTypes
                .SelectMany(openType => allTypes
                    .Where(t => t.IsClass &&
                        false == t.IsAbstract &&
                        t.AsType().ImplementsGenericInterface(openType))
                    )
                )
            {
                services.AddTransient(type.AsType());
            }

            var profiles = assemblies
               .SelectMany(t => t.GetTypes())
               .Distinct()
               .Where(t =>
                    true == t.IsClass &&
                    false == t.IsAbstract &&
                    false == t.IsInterface &&
                    true == t.IsPublic &&
                    typeof(AutoMapper.Profile).IsAssignableFrom(t) &&
                    t.Namespace != nameof(AutoMapper)
               );

            services.AddSingleton<AutoMapper.IConfigurationProvider>(p =>
                new AutoMapper.MapperConfiguration(cfg =>
                {
                    //cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                    //cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                    //cfg.CreateMissingTypeMaps = true;
                    cfg.AllowNullCollections = true;
                    cfg.AllowNullDestinationValues = true;
                    //cfg.ValidateInlineMaps = false;
                    foreach (var profile in p.GetService<IEnumerable<AutoMapper.Profile>>())
                    {
                        cfg.AddProfile(profile);
                    }

                    foreach (var profile in profiles)
                    {
                        cfg.AddProfile(
                           Activator.CreateInstance(profile)
                           as AutoMapper.Profile
                       );
                    }
                }));

            services.Add(new ServiceDescriptor(typeof(T), p =>
                new AutoMapper.Mapper(p.GetRequiredService<AutoMapper.IConfigurationProvider>(), p.GetService), lifetime));

            services.Add(new ServiceDescriptor(typeof(IMapperMgr), p =>
                new AutoMapperProvider(p.GetRequiredService<AutoMapper.IMapper>()), lifetime));

            return services;
        }

        static bool ImplementsGenericInterface(this Type type, Type interfaceType) =>
            type.IsGenericType(interfaceType) ||
            type.GetTypeInfo().ImplementedInterfaces
                .Any(@interface => @interface.IsGenericType(interfaceType));

        static bool IsGenericType(this Type type, Type genericType) =>
            type.GetTypeInfo().IsGenericType &&
            type.GetGenericTypeDefinition() == genericType;
    }
}
