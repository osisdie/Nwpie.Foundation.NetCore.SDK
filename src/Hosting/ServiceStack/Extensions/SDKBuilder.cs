using System;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Text;
using ServiceStack.Validation;
using Config = ServiceStack.Text.Config;
using HtmlFormat = ServiceStack.Formats.HtmlFormat;

namespace Nwpie.Foundation.Hosting.ServiceStack.Extensions
{
    public static class SDKBuilder
    {
        public static void Initialize()
        {
            ServiceContext.Initialize();

            Console.WriteLine(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddTraceData().DumpFormat());

            //if (true == ServiceContext.ServiceDiscoveryEnabled)
            //{
            //    Console.WriteLine($"Try register to {ServiceContext.ServiceDiscoveryUrl}");
            //}
        }

        public static ILoggingBuilder SdkLogging(this ILoggingBuilder builder, ILoggerProvider provider = null)
        {
            if (ServiceContext.IsDebugOrDevelopment())
            {
                builder.AddConsole();
            }

            if (null != provider)
            {
                LogMgr.LoggerFactory.AddProvider(provider);
            }

            return builder;
        }

        public static void SdkEnv(this IConfiguration configuration, IHostEnvironment env = null, ILoggerFactory loggerFactory = null)
        {
            if (null != configuration)
            {
                ServiceContext.Configuration = configuration;
            }

            if (null != env)
            {
                ServiceContext.HostingEnv = env;
            }

            if (null != loggerFactory)
            {
                LogMgr.LoggerFactory = loggerFactory;
            }
        }

        public static IApplicationBuilder UseSdk(this IApplicationBuilder app,
            string serviceName,
            IHostApplicationLifetime lifetime)
        {
            ServiceContext.ServiceName = serviceName;

            ComponentMgr.Instance.LifetimeScope = app
                .ApplicationServices
                .GetAutofacRoot();

            HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            return app;
            //return app.RegisterMyLocation(serviceName, lifetime);
        }

        public static void SdkServiceStackConfiguration(this AppHostBase host)
        {
            var hostCfg = new HostConfig
            {
                DebugMode = ServiceContext.IsDebugOrDevelopment(),
                // GlobalResponseHeaders = {},
                // MapExceptionToStatusCode = {},
            };

            var cors = new CorsFeature(
                allowCredentials: true,
                allowedMethods: "GET,POST,OPTIONS",
                allowedHeaders: "Accept,Origin,Content-Type,Allow,Authorization",
                allowedOrigins: "*"
            );

            if (ServiceContext.IsDebugOrDevelopment())
            {
                host.Plugins.Add(new PostmanFeature()
                {
                    Headers = "Accept: application/json\nContent-Type: application/json\nAuthorization: Bearer {{auth}}",
                    DefaultVerbsForAny = new List<string> { ServiceClientBase.DefaultHttpMethod },
                    FriendlyTypeNames = {
                        { "DateTime", "Date" }
                    }
                });
            }
            else
            {
                hostCfg.DebugMode = false;
                host.Plugins.RemoveAll(x => x is MetadataFeature);
            }

            host.Plugins.RemoveAll(x => x is HtmlFormat);

            JsConfig.Init(new Config()
            {
                //DateHandler = DateHandler.ISO8601,
                TextCase = TextCase.CamelCase,
            });

            // ISO8601
            JsConfig<DateTime?>.SerializeFn = o =>
                o.HasValue ? o.Value.ToString("s") : "";

            host.SetConfig(hostCfg);
            host.Plugins.Add(cors);
            host.Plugins.Add(new ValidationFeature());
        }

        public static void AutofacAdapter(this Funq.Container container)
        {
            IContainerAdapter adapter = new AutofacIocAdapter(ComponentMgr.Instance.DIContainer);
            container.Register<Autofac.ILifetimeScope>((c) => ComponentMgr.Instance.DIContainer.BeginLifetimeScope())
                .ReusedWithin(Funq.ReuseScope.Request);

            container.Adapter = adapter;
        }
    }
}
