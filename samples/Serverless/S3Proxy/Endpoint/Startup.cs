﻿using System.Text.Json;
using Autofac;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
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
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Cache.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.Foundation.DataAccess.Mapper.Extensions;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.Location.SDK.Extensions;
using Nwpie.Foundation.Measurement.SDK.Extensions;
using Nwpie.Foundation.MessageQueue.Factory.Extensions;
using Nwpie.Foundation.MessageQueue.SQS.Extensions;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Nwpie.Foundation.Notification.Contract;
using Nwpie.Foundation.Notification.SDK.Extensions;
using Nwpie.Foundation.S3Proxy.Contract;
using Nwpie.Foundation.S3Proxy.Endpoint.App_Start;
using Nwpie.Foundation.S3Proxy.Endpoint.Auth;
using Nwpie.Foundation.S3Proxy.Endpoint.Handlers;
using Nwpie.Foundation.ServiceNode.HealthCheck.Services;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.Storage.S3.Extensions;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack;
using IRepository = Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces.IRepository;

namespace Nwpie.Foundation.S3Proxy.Endpoint
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            configuration.SdkEnv(env);
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            // SDK
            services.AddLocationService<ILocationClient>();
            services.AddConfigServer<IConfigClient>();
            services.AddMetricService<IMeasurement>();
            services.AddDefaultSerializer<ISerializer>();
            services.AddLocalCache<ILocalCache>();
            services.AddAsDefaultICache<ILocalCache>();
            services.AddDefaultS3Option<AwsS3_Option>(
                configKey: S3ProxyServiceConfig.BucketConfigKeyForQC
            );
            services.AddDefaultS3Service<IS3StorageClient>(
                configKey: S3ProxyServiceConfig.BucketConfigKeyForQC
            );
            services.AddS3Factory<IAwsS3Factory>();
            services.AddDefaultAuthOption<Auth_Option>(
                configKey: SysConfigKey.Default_Auth_ConfigKey
            );
            services.AddApiKeyAuthService<IApiKeyAuthService, TokenDataModel>();
            services.AddJwtAuthService<IJwtAuthService, TokenDataModel>();
            //services.AddAesAuthService<IAesAuthService, TokenDataModel>();
            services.AddSingleton(typeof(IAesAuthService), p =>
               new TodoAesTokenService(
                  p.GetService<IConfigOptions<Auth_Option>>(),
                  p.GetService<ISerializer>(),
                  p.GetService<ICache>() //GetDefaultCacheFromConfig
              )
            );
            services.AddDefaultAuthService<ITokenService, IJwtAuthService>();
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
                typeof(HlckEcho_Repository).Assembly
            );
            services.RegisterAssemblyTypes<IDomainService>(ServiceLifetime.Transient,
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
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac
            //builder.RegisterModule<CustomServiceModule>();
            ComponentMgr.Instance.DIBuilder = builder;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
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
            app.UseSdk(NotificationServiceConfig.ServiceName,
                lifetime
            ).RegisterEvent(LocationConst.LocationEventName,
                LocationEventHandler.Instance.OnConsumed
            );
        }

        public const string AppS3BucketKey = "AppS3Bucket";
        public const string AppAuthUrlKey = "AppAuthUrl";
        public const string S3ProxyUrlKey = "S3_PROXY_URL"; // Env first
    }
}
