using System;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nwpie.Foundation.Abstractions.Statics
{
    public class SdkRuntime
    {
        public static bool IsEnvironment(EnvironmentEnum env) =>
            string.Equals(env.ToString(),
                ASPNETCORE_ENVIRONMENT,
                StringComparison.OrdinalIgnoreCase
            );

        public static bool IsDebugOrDevelopment() =>
            IsDebug || IsDevelopment();

        public static bool IsDevelopment() =>
            string.Equals(EnvironmentEnum.Development.ToString(),
                ASPNETCORE_ENVIRONMENT,
                StringComparison.OrdinalIgnoreCase
            );

        public static bool IsProduction() =>
            string.Equals(EnvironmentEnum.Production.ToString(),
                ASPNETCORE_ENVIRONMENT,
                StringComparison.OrdinalIgnoreCase
            );

        public static bool IsStaging() =>
            string.Equals(EnvironmentEnum.Staging.ToString(),
                ASPNETCORE_ENVIRONMENT,
                StringComparison.OrdinalIgnoreCase
            );

        public static bool IsPreproduction() =>
            string.Equals(EnvironmentEnum.Staging_2.ToString(),
                ASPNETCORE_ENVIRONMENT,
                StringComparison.OrdinalIgnoreCase
            );

        public static bool IsLocalConfigFirst() =>
            true == Config.LocalCfg?.Enabled &&
            true == Config.LocalCfg?.LocalConfigFilePath.HasValue();

        public static bool IsLocationAsConfigServer() =>
            true == Config.LOC?.AsConfigServerEnabled;

        public static IConfiguration Configuration { get; set; }

        static string m_Version;
        public static string Version
        {
            get => m_Version
                ?? Config.Version
                ?? new Version(1, 0, 0, 0).ToString();
            set => m_Version = value;
        }

        static string m_MachineName;
        public static string MachineName
        {
            get
            {
                if (null != m_MachineName)
                {
                    return m_MachineName;
                }

                try
                {
                    m_MachineName = Environment.GetEnvironmentVariable(SysVarConst.MachineName)
                        ?? Environment.MachineName;
                }
                catch { m_MachineName = ""; }

                return m_MachineName;
            }
        }

        static string m_ServiceName;
        public static string ServiceName
        {
            get => m_ServiceName ?? ApiName;
            set => m_ServiceName = value;
        }

        static string m_ApiBaseUrl;
        public static string ApiBaseUrl
        {
            get => m_ApiBaseUrl ?? Config.API?.BaseUrl;
            set => m_ApiBaseUrl = value;
        }

        static string m_ApiBearToken;
        public static string ApiBearToken
        {
            get => m_ApiBearToken ?? Config.API?.BearToken;
            set => m_ApiBearToken = value;
        }

        static string m_ApiKey;
        public static string ApiKey
        {
            get => m_ApiKey ?? Config.ApiKey;
            set => m_ApiKey = value;
        }

        static string m_ApiName;
        public static string ApiName
        {
            get => m_ApiName ?? Config.ApiName;
            set => m_ApiName = value;
        }

        static string m_ConfigServiceUrl;
        public static string ConfigServiceUrl
        {
            get => m_ConfigServiceUrl ?? Config.ConfigServiceUrl;
            set => m_ConfigServiceUrl = value;
        }

        static string m_LocationServiceUrl;
        public static string LocationServiceUrl
        {
            get => m_LocationServiceUrl ?? Config.LocationServiceUrl;
            set => m_LocationServiceUrl = value;
        }

        static string m_AuthServiceUrl;
        public static string AuthServiceUrl
        {
            get => m_AuthServiceUrl ?? Config.AuthServiceUrl;
            set => m_AuthServiceUrl = value;
        }

        static string m_NotifyServiceUrl;
        public static string NotifyServiceUrl
        {
            get => m_NotifyServiceUrl ?? Config.NotifyServiceUrl;
            set => m_NotifyServiceUrl = value;
        }

        static bool? m_MeasurementTraceEnabled;
        public static bool MeasurementTraceEnabled
        {
            get
            {
                if (m_MeasurementTraceEnabled.HasValue)
                {
                    return m_MeasurementTraceEnabled.Value;
                }

                m_MeasurementTraceEnabled = true == Config.APM?.MeasurementEnabled;
                return m_MeasurementTraceEnabled.Value;
            }
        }

        static bool? m_ServiceDiscoveryEnabled;
        public static bool ServiceDiscoveryEnabled
        {
            get
            {
                if (m_ServiceDiscoveryEnabled.HasValue)
                {
                    return m_ServiceDiscoveryEnabled.Value;
                }

                m_ServiceDiscoveryEnabled = true == Config.LOC?.ServiceDiscoveryEnabled;
                return m_ServiceDiscoveryEnabled.Value;
            }
        }

        static string m_ServiceDiscoveryUrl;
        public static string ServiceDiscoveryUrl
        {
            get
            {
                if (null != m_ServiceDiscoveryUrl)
                {
                    return m_ServiceDiscoveryUrl;
                }

                m_ServiceDiscoveryUrl = Config.LOC?.ServiceDiscoveryHostUrl ?? "localhost:8500";
                return m_ServiceDiscoveryUrl;
            }
        }

        static bool? m_ListenLocationEventEnabled;
        public static bool ListenLocationEventEnabled
        {
            get
            {
                if (m_ListenLocationEventEnabled.HasValue)
                {
                    return m_ListenLocationEventEnabled.Value;
                }

                m_ListenLocationEventEnabled = true == Config.LOC?.ListenLocationEventEnabled;
                return m_ListenLocationEventEnabled.Value;
            }
        }

        public static string SdkEnv = EnvironmentEnum.Production.GetDisplayName();

        public static string ASPNETCORE_ENVIRONMENT = "";
        public static bool IsDebug = false;

        // Align ASPNETCORE_ENVIRONMENT value
        public static IHostEnvironment HostingEnv;

        // Service up time since _ts
        public static readonly DateTime _ts = DateTime.UtcNow;

        // platform.json is necessary,
        // while platform.{ASPNETCORE_ENVIRONMENT}.json is {IsEnvConfigOptional}
        public static bool IsEnvConfigOptional = true;
        public static string IP => NetworkUtils.IP;

        public static readonly CloudProfile_Option AwsProfile = new CloudProfile_Option();
        public static readonly ServiceConfiguration Config = new ServiceConfiguration();

    }
}
