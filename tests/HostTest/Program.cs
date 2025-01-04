using System;
using System.IO;
using Nwpie.HostTest.App_Start;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace Nwpie.HostTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory((context) =>
                {
                    var options = new ServiceProviderOptions()
                    {
                        ValidateOnBuild = false, // default,
                        ValidateScopes = context.HostingEnvironment.IsDevelopment(),
                    };
                    return new CustomServiceProviderFactory(options);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIISIntegration();
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                        .AddEnvironmentVariables();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    var pathLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        hostingContext.Configuration.GetLogOutput(),
                        hostingContext.HostingEnvironment.ApplicationName,
                        "concurrentFlat.json"
                    );

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        loggerConfiguration
                          .MinimumLevel.Debug()
                          .Enrich.FromLogContext()
                          .Enrich.WithMachineName()
                          .Enrich.WithEnvironmentUserName()
                          .Enrich.WithProcessId()
                          .Enrich.WithProcessName()
                          .Enrich.WithThreadId()
                          .Enrich.WithProperty("ApplicationName", hostingContext.HostingEnvironment.ApplicationName)
                          .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment.EnvironmentName)
                          .WriteTo.Console()
                          .WriteTo.File(
                              path: pathLogFile,
                              rollingInterval: RollingInterval.Day,
                              formatter: new CompactJsonFormatter(),
                              rollOnFileSizeLimit: true,
                              fileSizeLimitBytes: 10000000
                          );
                    }
                    else
                    {
                        loggerConfiguration
                             .MinimumLevel.Information()
                             .Enrich.FromLogContext()
                             .Enrich.WithMachineName()
                             .Enrich.WithEnvironmentUserName()
                             .Enrich.WithProcessId()
                             .Enrich.WithProcessName()
                             .Enrich.WithThreadId()
                             .Enrich.WithProperty("ApplicationName", hostingContext.HostingEnvironment.ApplicationName)
                             .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment.EnvironmentName)
                             .WriteTo.File(
                                 path: pathLogFile,
                                 formatter: new CompactJsonFormatter(),
                                 rollOnFileSizeLimit: true,
                                 fileSizeLimitBytes: 10000000
                             );
                    }
                });
    }

    public static class ConfigurationExtension
    {
        public static string GetLogOutput(this IConfiguration config) => "log";
    }
}
