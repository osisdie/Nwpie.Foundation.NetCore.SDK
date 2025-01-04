using System;
using Nwpie.Foundation.Common;
//using Nwpie.Foundation.Common.Extensions;
//using Nwpie.Foundation.Common.Logging;
//using Nwpie.Foundation.Common.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nwpie.Foundation.Abstractions.Logging;
using Consul;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Hosting.ServiceStack.Extensions
{
    public static class ConsulExtension
    {
        static ConsulExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(ApplicationBuilderExtensions));
            if (ServiceContext.ServiceDiscoveryEnabled &&
                ServiceContext.ServiceDiscoveryUrl.HasValue())
            {
                try
                {
                    m_ConsulClient = new ConsulClient(x =>
                        x.Address = new Uri(ServiceContext.ServiceDiscoveryUrl)
                    );
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }

        public static void RegisterWithConsul(this IApplicationBuilder app,
            string serviceName,
            IHostApplicationLifetime lifetime = null)
        {
            if (null == m_ConsulClient || serviceName.IsNullOrEmpty())
            {
                return;
            }

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://localhost/health",
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = ServiceContext.ApiName,
                Address = NetworkUtils.IP,
                Port = 80,
                Tags = new[] {
                    serviceName,
                    ServiceContext.ApiName,
                    ServiceContext.SdkEnv,
                    ServiceContext.ASPNETCORE_ENVIRONMENT
                }
            };

            m_ConsulClient.Agent.ServiceRegister(registration).Wait();

            if (null != lifetime)
            {
                lifetime.ApplicationStopping.Register(() =>
                {
                    try
                    {
                        if (null != m_ConsulClient?.Agent)
                        {
                            m_ConsulClient.Agent.ServiceDeregister(registration.ID).Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }
                });
            }
        }

        static readonly ILogger Logger;
        static readonly IConsulClient m_ConsulClient;
    }
}
