using System;
using System.IO;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nwpie.Foundation.Common
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static class ServiceContext
    {
        static ServiceContext()
        {
            ReadSdkEnvironment();
            ReadVersionFile();
        }

        public static void Initialize(string platformFileName = ConfigConst.DefaultPlatformConfigFile)
        {
            if (false == m_IsInitialized)
            {
                lock (m_Lock)
                {
                    if (false == m_IsInitialized)
                    {
                        IConfigurationBuilder builder = new ConfigurationBuilder();
                        if (platformFileName.HasValue())
                        {
                            var optional = false;
                            Console.WriteLine($"Loading {platformFileName}, optional:{optional} ");
                            builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile(platformFileName);

                            if (ASPNETCORE_ENVIRONMENT.HasValue())
                            {
                                optional = true;
                                var envConfig = $"{Path.GetFileNameWithoutExtension(platformFileName)}.{ ASPNETCORE_ENVIRONMENT}{Path.GetExtension(platformFileName)}";
                                builder.AddJsonFile(envConfig, optional: optional);
                                Console.WriteLine($"Loading {envConfig}, optional:{optional}");
                            }
                        }

                        var config = builder.Build();
                        config.Bind(ConfigConst.DefaultFoundationSectionName, Config);

                        IsDebug = SdkEnv == EnvironmentEnum.Testing.GetDisplayName() ||
                            SdkEnv == EnvironmentEnum.Debug.GetDisplayName();

                        if (ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), ApiKey, ApiName, ConfigServiceUrl))
                        {
                            throw new ArgumentNullException($@"SDK initialization arguments
                                {nameof(ApiKey)},
                                {nameof(ApiName)},
                                {nameof(ConfigServiceUrl)} are required. ");
                        }

                        m_IsInitialized = true;
                    }
                }
            }
        }

        public static void ReadVersionFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigConst.DefaultVersionFile);
            if (File.Exists(path))
            {
                try
                {
                    Version = File.ReadAllText(path).Trim();
                }
                catch { }
            }
        }

        public static void ReadSdkEnvironment()
        {
            // Align Microsoft, default: `Production`
            // => platform.Production.json must exist
            // => Limited error message detail
            // => Hide metadata or swagger documents
            ASPNETCORE_ENVIRONMENT = Environment
                .GetEnvironmentVariable(SysVarConst.AspNetCoreEnvironment)
                ?.Trim();
            // Default: Production
            var parsedEnv = Enum<EnvironmentEnum>
                .TryParse(ASPNETCORE_ENVIRONMENT, EnvironmentEnum.Max);
            if (EnvironmentEnum.Max == parsedEnv)
            {
                ASPNETCORE_ENVIRONMENT = EnvironmentEnum.Production.ToString();
                SdkEnv = EnvironmentEnum.Production.GetDisplayName();
            }
            else
            {
                SdkEnv = parsedEnv.GetDisplayName();
            }

            // Required
            ApiKey = Environment
                .GetEnvironmentVariable(SysVarConst.SdkApiKey)
                ?.Trim()
                ?? throw new ArgumentNullException($"Environment.GetEnvironmentVariable({SysVarConst.SdkApiKey})");

            ApiName = Environment
                .GetEnvironmentVariable(SysVarConst.SdkApiName)
                ?.Trim()
                ?? throw new ArgumentNullException($"Environment.GetEnvironmentVariable({SysVarConst.SdkApiKey})");

            ApiBaseUrl = Environment
                .GetEnvironmentVariable(SysVarConst.SdkApiBaseUrl)
                ?.Trim()
                ?? throw new ArgumentNullException($"Environment.GetEnvironmentVariable({SysVarConst.SdkApiBaseUrl})");

            ConfigServiceUrl = Environment
                .GetEnvironmentVariable(SysVarConst.SdkConfigServiceUrl)
                ?.Trim()
                ?? throw new ArgumentNullException($"Environment.GetEnvironmentVariable({SysVarConst.SdkConfigServiceUrl})");

            #region Optional
            ApiBearToken = Environment
                .GetEnvironmentVariable(SysVarConst.SdkApiBearerToken)
                ?.Trim();

            LocationServiceUrl = Environment
                .GetEnvironmentVariable(SysVarConst.SdkLocServiceUrl)
                ?.Trim();

            AuthServiceUrl = Environment
                .GetEnvironmentVariable(SysVarConst.SdkAuthServiceUrl)
                ?.Trim();

            NotifyServiceUrl = Environment
               .GetEnvironmentVariable(SysVarConst.SdkNotifyServiceUrl)
               ?.Trim();

            /// AWS Ordering:
            /// 1.Environment Variable
            /// 2.~/.aws/Credentials
            /// 3.ECS container credentials
            /// 4.Instance profile
            AwsProfile.AccessKey = Environment
                .GetEnvironmentVariable(SysVarConst.AwsAccessKeyId)
                ?.Trim();

            AwsProfile.SecretKey = Environment
                .GetEnvironmentVariable(SysVarConst.AwsSecretAccessKey)
                ?.Trim();
            #endregion
        }

        public static bool IsEnvironment(EnvironmentEnum env) => SdkRuntime.IsEnvironment(env);
        public static bool IsDebugOrDevelopment() => SdkRuntime.IsDebugOrDevelopment();
        public static bool IsDevelopment() => SdkRuntime.IsDevelopment();
        public static bool IsProduction() => SdkRuntime.IsProduction();
        public static bool IsStaging() => SdkRuntime.IsStaging();
        public static bool IsPreproduction() => SdkRuntime.IsPreproduction();
        public static bool IsLocalConfigFirst() => SdkRuntime.IsLocalConfigFirst();
        public static bool IsLocationAsConfigServer() => SdkRuntime.IsLocationAsConfigServer();

        public static IConfiguration Configuration
        {
            get => SdkRuntime.Configuration;
            set => SdkRuntime.Configuration = value;
        }

        public static string ServiceName
        {
            get => SdkRuntime.ServiceName;
            set => SdkRuntime.ServiceName = value;
        }

        public static string ApiBaseUrl
        {
            get => SdkRuntime.ApiBaseUrl;
            set => SdkRuntime.ApiBaseUrl = value;
        }

        public static string ApiBearToken
        {
            get => SdkRuntime.ApiBearToken;
            set => SdkRuntime.ApiBearToken = value;
        }

        public static string ApiKey
        {
            get => SdkRuntime.ApiKey;
            set => SdkRuntime.ApiKey = value;
        }

        public static string ApiName
        {
            get => SdkRuntime.ApiName;
            set => SdkRuntime.ApiName = value;
        }

        public static string ConfigServiceUrl
        {
            get => SdkRuntime.ConfigServiceUrl;
            set => SdkRuntime.ConfigServiceUrl = value;
        }

        public static string LocationServiceUrl
        {
            get => SdkRuntime.LocationServiceUrl;
            set => SdkRuntime.LocationServiceUrl = value;
        }

        public static string AuthServiceUrl
        {
            get => SdkRuntime.AuthServiceUrl;
            set => SdkRuntime.AuthServiceUrl = value;
        }

        public static string NotifyServiceUrl
        {
            get => SdkRuntime.NotifyServiceUrl;
            set => SdkRuntime.NotifyServiceUrl = value;
        }

        public static CloudProfile_Option AwsProfile => SdkRuntime.AwsProfile;
        public static ServiceConfiguration Config => SdkRuntime.Config;
        public static bool MeasurementTraceEnabled => SdkRuntime.MeasurementTraceEnabled;
        public static bool ServiceDiscoveryEnabled => SdkRuntime.ServiceDiscoveryEnabled;
        public static string ServiceDiscoveryUrl => SdkRuntime.ServiceDiscoveryUrl;
        public static bool ListenLocationEventEnabled => SdkRuntime.ListenLocationEventEnabled;
        public static string MachineName => SdkRuntime.MachineName;
        public static bool IsEnvConfigOptional => SdkRuntime.IsEnvConfigOptional;

        public static string Version
        {
            get => SdkRuntime.Version;
            set => SdkRuntime.Version = value;
        }

        public static string SdkEnv
        {
            get => SdkRuntime.SdkEnv;
            set => SdkRuntime.SdkEnv = value;
        }
        public static string ASPNETCORE_ENVIRONMENT
        {
            get => SdkRuntime.ASPNETCORE_ENVIRONMENT;
            set => SdkRuntime.ASPNETCORE_ENVIRONMENT = value;
        }
        public static bool IsDebug
        {
            get => SdkRuntime.IsDebug;
            set => SdkRuntime.IsDebug = value;
        }

        public static IHostEnvironment HostingEnv
        {
            get => SdkRuntime.HostingEnv;
            set => SdkRuntime.HostingEnv = value;
        }

        public static DateTime _ts => SdkRuntime._ts;

        private static readonly object m_Lock = new object();
        private static bool m_IsInitialized = false;
    }
}
