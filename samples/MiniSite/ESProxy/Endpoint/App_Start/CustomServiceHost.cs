using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.ServiceNode.HealthCheck;
using Nwpie.MiniSite.ES.Contract;
using Nwpie.MiniSite.ES.Endpoint.Handlers;
using ServiceStack;
using ServiceStack.Api.OpenApi;

namespace Nwpie.MiniSite.ES.Endpoint.App_Start
{
    /// <summary>
    /// Create your ServiceStack web service application with a singleton AppHost.
    /// </summary>
    internal sealed class CustomServiceHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public CustomServiceHost() : base(ESProxyServiceConfig.ServiceName,
            typeof(CustomServiceHost).Assembly,
            typeof(HlckEcho_Service).Assembly)
        {
        }

        public override RouteAttribute[] GetRouteAttributes(Type requestType)
        {
            var routes = base.GetRouteAttributes(requestType);
            //routes.Each(x => x.Path = "/api" + x.Path);

            return routes;
        }

        /// <summary>
        /// Configure the container with the necessary routes for your ServiceStack application.
        /// </summary>
        /// <param name="container">The built-in IoC used with ServiceStack.</param>
        public override void Configure(Funq.Container container)
        {
            this.SdkServiceStackConfiguration();
            if (ServiceContext.IsDebugOrDevelopment())
            {
                Plugins.Add(new OpenApiFeature());
            }

            // Extra event handling
            this.AddGlobalPreRequestFilters();
            this.AddGlobalRequestFilters();
            //this.AddGlobalResponseFilters();
            this.AddServiceExceptionHandlers();
            this.AddUncaughtExceptionHandlers();

            var indices = SysConfigKey
                .Whitelist_ElasticSearch_Indices_ConfigKey
                .ConfigServerRawValue();
            if (null != indices)
            {
                ESProxyServiceConfig.Indices = ComponentMgr.Instance
                    .GetDefaultSerializer()
                    ?.Deserialize<List<string>>(indices, ignoreException: true);
            }

            ESProxyServiceConfig.ElasticSearchBaseUrl = SysConfigKey
                .Default_ElasticSearch_HostUrl_ConfigKey
                .ConfigServerRawValue()
                ?.TrimEndSlash();
            var proxy = new ProxyFeature(
                matchingRequests: req =>
                    true != req.RawUrl.IsHealthCheckRequest(),
                resolveUrl: req =>
                    ESProxyServiceConfig.ElasticSearchBaseUrl + req.PathInfo
            );

            Plugins.Add(proxy);

            container.AutofacAdapter();
        }
    }
}
