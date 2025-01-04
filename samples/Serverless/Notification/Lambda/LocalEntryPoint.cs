using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.Foundation.Hosting.ServiceStack.Extensions;
using Nwpie.Foundation.Notification.Lambda.Service.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nwpie.Foundation.Notification.Lambda.Service
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        public static async Task Main(string[] args)
        {
            SDKBuilder.Initialize();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging((logging, builder) =>
                {
                    builder.SdkLogging(new Log4netProvider(LoggingUtils.EnvironmentLog4netFile));
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables()
                        .SdkConfiguration();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
