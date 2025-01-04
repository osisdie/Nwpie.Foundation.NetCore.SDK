﻿using System;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.Location.Contract;
using Nwpie.Foundation.Location.Endpoint.Handlers;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation;
using Nwpie.Foundation.ServiceNode.HealthCheck;
using ServiceStack;
using ServiceStack.Api.OpenApi;

namespace Nwpie.Foundation.Location.Endpoint.App_Start
{
    /// <summary>
    /// Create your ServiceStack web service application with a singleton AppHost.
    /// </summary>
    internal sealed class CustomServiceHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public CustomServiceHost() : base(LocationServiceConfig.ServiceName,
            typeof(LocGetApiLocation_Service).Assembly,
            typeof(HlckEcho_Service).Assembly)
        {
            // 30 Day Free Trial
            //Licensing.RegisterLicense("*****");
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
            //this.AddGlobalPreRequestFilters();
            this.AddGlobalRequestFilters();
            //this.AddGlobalResponseFilters();
            this.AddServiceExceptionHandlers();
            this.AddUncaughtExceptionHandlers();

            container.AutofacAdapter();
        }
    }
}
