using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.Foundation.DataAccess.Mapper
{
    [Obsolete("User AutoMapperProvider")]
    public class AutoMapperMgr : CObject//, IMapperMgr
    {
        public AutoMapperMgr() : base() { }

        public void AddAutoMapperService(Autofac.ContainerBuilder builder)
        {
            builder.Register(c => new AutoMapper.MapperConfiguration(cfg =>
            {
                //cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                //cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.CreateMissingTypeMaps = true;
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                //cfg.ValidateInlineMaps = false;
                foreach (var profile in c.Resolve<IEnumerable<AutoMapper.Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().AutoActivate().SingleInstance();

            builder.Register(c =>
                c.Resolve<AutoMapper.MapperConfiguration>()
                .CreateMapper(c.Resolve)
            ).As<IMapper>().AsImplementedInterfaces().SingleInstance();
        }

        public void AddAutoMapperService(params Assembly[] assembliesWithProfiles)
        {
            var profiles = assembliesWithProfiles
                .SelectMany(t => t.GetTypes())
                .Distinct()
                .Where(t => typeof(AutoMapper.Profile).IsAssignableFrom(t) &&
                    false == t.IsInterface &&
                    t.IsPublic
                );

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                //cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                //cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.CreateMissingTypeMaps = true;
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                //cfg.ValidateInlineMaps = false;
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(
                        Activator.CreateInstance(profile)
                        as AutoMapper.Profile
                    );
                }
            });

            Mapper = config.CreateMapper();
        }

        public TDestination ConvertTo<TDestination>(object source) =>
            Mapper.Map<TDestination>(source);

        public IEnumerable<TDestination> ConvertAll<TDestination>(IEnumerable<object> source) =>
            source
            ?.ToList()
            ?.ConvertAll(o => ConvertTo<TDestination>(o));

        public TDestination ConvertTo<TSource, TDestination>(TSource source) =>
            (Mapper ?? ComponentMgr.Instance.TryResolve<IMapper>())
            .Map<TSource, TDestination>(source);

        public IEnumerable<TDestination> ConvertAll<TSource, TDestination>(IEnumerable<TSource> source) =>
            source
            ?.ToList()
            ?.ConvertAll(o => ConvertTo<TSource, TDestination>(o));

        public void Dispose() { }

        /// <summary>
        /// The config for CreateMapper
        /// </summary>
        public IMapper Mapper { get; private set; }
    }
}
