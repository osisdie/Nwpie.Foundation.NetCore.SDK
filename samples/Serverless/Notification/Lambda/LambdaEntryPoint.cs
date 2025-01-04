using System;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Nwpie.Foundation.Notification.Lambda.Service
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the
    /// actual Lambda function entry point. The Lambda handler field should be set to
    ///
    /// Nwpie.Foundation.Notification.Lambda.Service::Nwpie.Foundation.Notification.Lambda.Service.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :
        // When using an ELB's Application Load Balancer as the event source change
        // the base class to Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        /// <summary>
        /// The builder has configuration, logging and Amazon API Gateway already configured. The startup class
        /// needs to be configured in this method using the UseStartup<>() method.
        /// </summary>
        /// <param name="builder"></param>
        protected override void Init(IWebHostBuilder builder)
        {
            SDKBuilder.Initialize();

            builder
                //.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                //.ConfigureLogging((logging, builder) =>
                //{
                //    builder.SdkLogging(new Log4netProvider(LoggingUtils.EnvironmentLog4netFile));
                //})
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddEnvironmentVariables()
                        .SdkConfiguration();
                })
                .UseStartup<Startup>();
        }
    }
}
