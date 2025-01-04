using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.S3Proxy.Lambda.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            //configuration.SdkEnv(env);
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddCors(options =>
                options.AddPolicy("AllowCors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                )
            );
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
                //opt.RespectBrowserAcceptHeader = true; // false by default
                //opt.Filters.Add(new ModelStateValidator());
            })
            .AddControllersAsServices()
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //opt.SerializerSettings.ContractResolver = new DefaultContractResolver
                //{
                //    NamingStrategy = new CamelCaseNamingStrategy()
                //};
                //opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            // SDK
            //services.AddConfigServer<IRemoteConfigClient>(
            //    client: Client_ConfigServer.Instance
            //);
            //services.AddMetricService<IMeasurement>();
            //services.AddDefaultSerializer<ISerializer>();
            //services.AddLocalCache<ILocalCache>();
            //services.AddAsDefaultICache<ILocalCache>();
            //services.AddDefaultS3Option<AwsS3_Option>(
            //    configKey: S3ProxyLambdaServiceConfig.BucketConfigKeyForQC
            //);
            //services.AddDefaultS3Service<IS3StorageClient>(
            //    configKey: S3ProxyLambdaServiceConfig.BucketConfigKeyForQC
            //);
            //services.AddS3Factory<IAwsS3Factory>();
            //services.AddDefaultAuthOption<Auth_Option>(
            //    configKey: SysConfigKey.Default_Auth_ConfigKey
            //);
            //services.AddApiKeyAuthService<IApiKeyAuthService, TokenDataModel>();
            //services.AddJwtAuthService<IJwtAuthService, TokenDataModel>();
            //services.AddSingleton(typeof(IAesAuthService), p =>
            //   new TodoAesTokenService(
            //      p.GetService<IConfigOptions<Auth_Option>>(),
            //      p.GetService<ISerializer>(),
            //      p.GetService<ICache>() //GetDefaultCacheFromConfig
            //  )
            //);
            //services.AddDefaultAuthService<ITokenService, IJwtAuthService>();
            //services.AddNotifyHttpClient<INotificationHttpClient>();
            //services.AddNotifySQSClient<INotificationSQSClient>();
            //services.AddMessageQueueFactory<IMessageQueueFactory>();
            //services.AddSQSService<IAwsSQSClient>(
            //    configKey: SysConfigKey.Default_AWS_SQS_Urls_Notification_ConfigKey
            //);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net($"conf/log4net.{env.EnvironmentName}.config");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                //routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public const string AppS3BucketKey = "AppS3Bucket";
        public const string AppAuthUrlKey = "AppAuthUrl";
    }
}
