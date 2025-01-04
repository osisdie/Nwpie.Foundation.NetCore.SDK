using System;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.S3Proxy.Contract;
using Nwpie.Foundation.S3Proxy.Endpoint.Handlers;
using Nwpie.Foundation.S3Proxy.Endpoint.ServiceCore.Upload;
using Nwpie.Foundation.ServiceNode.HealthCheck;
using ServiceStack;
using ServiceStack.Api.OpenApi;

namespace Nwpie.Foundation.S3Proxy.Endpoint.App_Start
{
    /// <summary>
    /// Create your ServiceStack web service application with a singleton AppHost.
    /// </summary>
    internal sealed class CustomServiceHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public CustomServiceHost() : base(S3ProxyServiceConfig.ServiceName,
            typeof(ImageUpload_Service).Assembly,
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
            this.AddServiceExceptionHandlers();
            this.AddUncaughtExceptionHandlers();

            S3ProxyServiceConfig.S3ServerlessBaseUrl = Startup
                .S3ProxyUrlKey
                .EnvFirstConfig()
                ?.TrimEndSlash();
            var proxy = new ProxyFeature(
                matchingRequests: req =>
                    true != req.RawUrl.IsHealthCheckRequest(),
                resolveUrl: req =>
                    S3ProxyServiceConfig.S3ServerlessBaseUrl + req.PathInfo
            );

            Plugins.Add(proxy);

            container.AutofacAdapter();
        }
    }
}
