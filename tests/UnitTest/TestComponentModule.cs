using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Caching.Redis;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Storage;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Nwpie.Foundation.DataAccess.Mapper;
using Nwpie.Foundation.Http.Common.Interfaces;
using Nwpie.Foundation.Location.SDK;
using Nwpie.Foundation.Measurement.SDK;
using Nwpie.Foundation.MessageQueue.Factory;
using Nwpie.Foundation.MessageQueue.SNS;
using Nwpie.Foundation.MessageQueue.SNS.Interfaces;
using Nwpie.Foundation.MessageQueue.SQS;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Nwpie.Foundation.Notification.SDK;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Models;
using Nwpie.Foundation.Storage.S3;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Consul;
using Microsoft.Extensions.Caching.Memory;
using Nwpie.xUnit.Models;

namespace Nwpie.xUnit
{
    public sealed class TestComponentModule : Autofac.Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            base.Load(builder);

            var componentMgr = ComponentMgr.Instance;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            assemblies.SelectMany(x => x.GetReferencedAssemblies())
                .Where(t => t.Name
                    .StartsWith($"{CommonConst.SdkPrefix}.", StringComparison.OrdinalIgnoreCase)
                )
                .Where(t => false == assemblies
                    .Any(a =>
                        a.FullName == t.FullName ||
                        a.GetName().Name == nameof(AutoMapper)
                    )
                )
                .Distinct()
                .ToList()
                .ForEach(x => assemblies.Add(AppDomain.CurrentDomain.Load(x)));

            var asm = assemblies.ToArray();

            //builder.RegisterGeneric(typeof(Logger<>))
            //    .As(typeof(ILogger<>)).SingleInstance();

            builder.RegisterType<LocationClient>()
                .As<ILocationClient>().SingleInstance();

            //builder.RegisterInstance(Client_ConfigServer.Instance)
            //    .As<IClient_ConfigServer>().SingleInstance();

            builder.RegisterType<DefaultRemoteConfigClient>()
                .As<IConfigClient>().SingleInstance();

            builder.RegisterType<MetricClient>()
                .As<IMeasurement>().SingleInstance();

            builder.RegisterType<DefaultHttpSerializer>()
                .As<IHttpSerializer>().SingleInstance();

            builder.RegisterType<DefaultJsonConfigSerializer>()
                .As<IJsonConfigSerializer>().SingleInstance();

            builder.RegisterType<DefaultEntitySerializer>()
                .As<IEntitySerializer>().SingleInstance();

            builder.RegisterType<DefaultSerializer>()
                .As<ISerializer>().SingleInstance();

            builder.RegisterInstance(new DefaultMemoryCache(
                new MemoryCache(new MemoryCacheOptions
                {
                    TrackStatistics = true
                })
            )).As<ILocalCache>().SingleInstance();

            builder.Register(x => new ConfigOptions<RedisCache_Option>(
                new RedisCache_Option()
                {
                    ConnectionString = SysConfigKey
                        .Default_AWS_Redis_ConnectionString_ConfigKey
                        .ConfigServerRawValue()
                }
            )).As(typeof(IConfigOptions<RedisCache_Option>)).SingleInstance();

            // Default Redis
            builder.Register(x => new RedisCache(
                componentMgr.TryResolve<IConfigOptions<RedisCache_Option>>()
            )).As<IRedisCache>().SingleInstance();

            // Default Cache, ILocalCache or IRedisCache
            builder.Register(x => componentMgr.TryResolve<ILocalCache>())
                .As<ICache>().SingleInstance();

            builder.Register(x => new ConfigOptions<AwsS3_Option>(
                SysConfigKey
                    .Default_AWS_S3_Credential_ConfigKey
                    .ConfigServerValue<AwsS3_Option>()
            )).As(typeof(IConfigOptions<AwsS3_Option>)).SingleInstance();

            builder.Register(x => new ConfigOptions<LocalStorage_Option>(
                new LocalStorage_Option()
                {
                    BasePath = AppDomain.CurrentDomain.BaseDirectory,
                    BucketName = ConfigConst.DefaultTempFolder
                }
            )).As(typeof(IConfigOptions<LocalStorage_Option>)).SingleInstance();

            builder.RegisterType<AwsS3Factory>()
                .As<IAwsS3Factory>().SingleInstance();

            builder.Register(x => new S3StorageClient(
                componentMgr.TryResolve<IConfigOptions<AwsS3_Option>>(),
                componentMgr.GetDefaultCache()
            )).As<IS3StorageClient>().SingleInstance();

            builder.Register(x => new LocalStorageClient(
                componentMgr.TryResolve<IConfigOptions<LocalStorage_Option>>(),
                componentMgr.GetDefaultCache()
            )).As<ILocalStorageClient>().SingleInstance();

            // IS3StorageClient or ILocalStorageClient
            builder.Register(x => componentMgr.TryResolve<IS3StorageClient>())
                .As<IStorage>().SingleInstance();

            Assembly[] converterAsm = new Assembly[] {
                typeof(Ds1SourceItem_Converter).Assembly,
                typeof(TestSnakeTable_Converter).Assembly,
            }.Distinct().ToArray();

            builder.RegisterAssemblyTypes(converterAsm)
                .PublicOnly()
                .Where(t => t.IsClass &&
                    !t.IsAbstract &&
                    typeof(AutoMapper.Profile).IsAssignableFrom(t))
                .As<AutoMapper.Profile>()
                .InstancePerLifetimeScope(); // We just need to initialize once per profile

            builder.Register(x => new AutoMapper.MapperConfiguration(cfg =>
            {
                //cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                //cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.CreateMissingTypeMaps = true;
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                //cfg.ValidateInlineMaps = false;
                foreach (var profile in x.Resolve<IEnumerable<AutoMapper.Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().AutoActivate().SingleInstance();

            builder.Register(x => x.Resolve<AutoMapper.MapperConfiguration>()
                .CreateMapper(x.Resolve)
            ).As<AutoMapper.IMapper>().SingleInstance();

            builder.Register(x => new AutoMapperProvider(
                x.Resolve<AutoMapper.IMapper>())
            ).As<IMapperMgr>().SingleInstance();

            builder.Register(x => new MessageQueueFactory(
                componentMgr.TryResolve<ISerializer>()
            )).As<IMessageQueueFactory>().SingleInstance();

            builder.Register(x => new NotificationHttpClient(
                SysConfigKey
                    .Default_Notification_HostUrl_ConfigKey
                    .ConfigServerRawValue(),
                componentMgr.TryResolve<ISerializer>()
            )).As<INotificationHttpClient>().SingleInstance();

            builder.Register(x => new NotificationSQSClient(
                new ConfigOptions<AwsSQS_Option>(
                    SysConfigKey
                        .Default_AWS_SQS_Urls_Notification_ConfigKey
                        .ConfigServerValue<AwsSQS_Option>()
                ), componentMgr.TryResolve<ISerializer>()
            )).As<INotificationSQSClient>().SingleInstance();

            builder.Register(x => new ConfigOptions<Auth_Option>(
                SysConfigKey
                    .Default_Auth_ConfigKey
                    .ConfigServerValue<Auth_Option>()
            )).As(typeof(IConfigOptions<Auth_Option>)).SingleInstance();

            builder.Register(x => new DefaultAesAuthService<TokenDataModel>(
                componentMgr.TryResolve<IConfigOptions<Auth_Option>>(),
                componentMgr.TryResolve<ISerializer>(),
                componentMgr.GetDefaultCacheFromConfig() //Resolve<ICache>()
            )).As<IAesAuthService>().SingleInstance();

            builder.Register(x => new DefaultJwtAuthService<TokenDataModel>(
                componentMgr.TryResolve<IConfigOptions<Auth_Option>>(),
                componentMgr.TryResolve<ISerializer>(),
                componentMgr.GetDefaultCacheFromConfig() //Resolve<ICache>()
            )).As<IJwtAuthService>().SingleInstance();

            // Default: JWT
            builder.Register(x => componentMgr
                .TryResolve<IJwtAuthService>() // ComponentMgr.Instance.GetDefaultCacheFromConfig()
            ).As<ITokenService>().SingleInstance();

            // Notification Queue
            builder.Register(x => new AwsSQSClient(
                new ConfigOptions<AwsSQS_Option>(
                    SysConfigKey
                        .Default_AWS_SQS_Urls_Notification_ConfigKey
                        .ConfigServerValue<AwsSQS_Option>()
                ), componentMgr.TryResolve<ISerializer>()
            )).As<IAwsSQSClient>().SingleInstance();

            // Broadcast
            builder.Register(x => new AwsNotificationClient(
                new ConfigOptions<AwsSNS_Option>(
                    SysConfigKey
                        .Default_AWS_SNS_Urls_Location_ConfigKey
                        .ConfigServerValue<AwsSNS_Option>()
                ), componentMgr.TryResolve<ISerializer>()
            )).As<IAwsNotificationClient>().SingleInstance();

            builder.Register(x => new ConsulClient(ctx =>
            {
                ctx.Address = new Uri(ServiceContext.ServiceDiscoveryUrl);
            })).As<IConsulClient>().SingleInstance();

            builder.RegisterType<EmptyRequestService>()
                .As<IRequestService>().AsImplementedInterfaces();

            // repository
            builder.RegisterAssemblyTypes(asm)
                .PublicOnly()
                .Where(t => false == t.IsInterface)
                .As<IRepository>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            // domain service
            builder.RegisterAssemblyTypes(asm)
               .PublicOnly()
               .Where(t => false == t.IsInterface)
               .Where(t => typeof(IDomainService).IsAssignableFrom(t))
               //.UsingConstructor(new DefaultConstructorSelector(1))
               .AsImplementedInterfaces()
               .InstancePerDependency();
        }
    }
}
