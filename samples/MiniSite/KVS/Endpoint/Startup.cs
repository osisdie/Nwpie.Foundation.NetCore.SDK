using System.Text.Json;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.MessageQueue.Interfaces;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Caching.Redis.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Cache.Extensions;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.DataAccess.Mapper.Extensions;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.Location.SDK.Extensions;
using Nwpie.Foundation.Measurement.SDK.Extensions;
using Nwpie.Foundation.MessageQueue.Factory.Extensions;
using Nwpie.Foundation.MessageQueue.SQS.Extensions;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Nwpie.Foundation.Notification.SDK.Extensions;
using Nwpie.Foundation.ServiceNode.HealthCheck.Services;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.Contract;
using Nwpie.MiniSite.KVS.Endpoint.App_Start;
using Nwpie.MiniSite.KVS.Endpoint.Handlers;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack;
using IRepository = Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces.IRepository;

namespace Nwpie.MiniSite.KVS.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            configuration.SdkEnv(env);
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            // SDK
            services.AddLocationService<ILocationClient>();
            services.AddMetricService<IMeasurement>();
            services.AddDefaultSerializer<ISerializer>();
            services.AddLocalCache<ILocalCache>();
            services.AddDefaultRedisConnectionString<RedisCache_Option>(
                conn: SysConfigKey
                    .Default_AWS_Redis_ConnectionString_ConfigKey
                    .ConfigServerRawValue()
            );
            services.AddRedisCache<IRedisCache>();
            services.AddAsDefaultICache<ILocalCache>();
            services.AddDefaultAuthOption<Auth_Option>(
                configKey: SysConfigKey.Default_Auth_ConfigKey
            );
            services.AddApiKeyAuthService<IApiKeyAuthService, TokenDataModel>();
            services.AddJwtAuthService<IJwtAuthService, TokenDataModel>();
            services.AddDefaultAuthService<ITokenService, IApiKeyAuthService>();
            services.AddMappers<AutoMapper.IMapper>();
            services.AddNotifyHttpClient<INotificationHttpClient>(
                configKey: SysConfigKey.Default_Notification_HostUrl_ConfigKey
            );
            services.AddNotifySQSClient<INotificationSQSClient, AwsSQS_Option>(
                configKey: SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey
            );
            services.AddMessageQueueFactory<IMessageQueueFactory>();
            services.AddSQSService<IAwsSQSClient>(
                configKey: SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey
            );
            //services.AddSNSService<IAwsNotificationClient>(
            //    configKey: SysConfigKey.Default_AWS_SNS_Urls_Location_ConfigKey
            //);
            services.RegisterAssemblyTypes<IRepository>(ServiceLifetime.Transient,
                typeof(KvsGet_Repository).Assembly,
                typeof(HlckEcho_Repository).Assembly
            );
            services.RegisterAssemblyTypes<IDomainService>(ServiceLifetime.Transient,
                typeof(KvsGet_DomainService).Assembly,
                typeof(HlckEcho_DomainService).Assembly
            );

            // MVC
            services.AddHealthChecks();
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
                opt.Filters.Add(new ModelStateValidator());
            })
            .AddControllersAsServices()
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(Autofac.ContainerBuilder builder)
        {
            // Register your own things directly with Autofac
            //builder.RegisterModule<CustomServiceModule>();
            ComponentMgr.Instance.DIBuilder = builder;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            if (ServiceContext.IsDebugOrDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });
            app.UseServiceStack(new CustomServiceHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
            app.UseSdk(KVServiceConfig.ServiceName,
                lifetime
            ).RegisterEvent(LocationConst.LocationEventName,
                LocationEventHandler.Instance.OnConsumed
            );
        }
    }
}
