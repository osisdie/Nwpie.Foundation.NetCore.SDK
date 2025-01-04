using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;

namespace Nwpie.Foundation.Abstractions.Config
{
    public class ServiceConfiguration : IOption
    {
        public string Version { get; set; }
        public string ApiToken { get; set; }
        public string ApiName { get; set; }
        public string ApiKey { get; set; }
        public string AuthServiceUrl { get; set; }
        public string NotifyServiceUrl { get; set; }
        public string ConfigServiceUrl { get; set; }
        public string LocationServiceUrl { get; set; }

        // (filePath, isOptional:false)
        public Dictionary<string, bool> StartupJsonFileList { get; set; } = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public List<string> RemoteConfigKeys { get; set; } = new List<string>();
        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, bool> FeatureToggle { get; set; } = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        public LocationConfigurationSection LOC { get; set; } = new LocationConfigurationSection();
        public ApiConfigurationSection API { get; set; } = new ApiConfigurationSection();
        public DalConfigurationSection DAL { get; set; } = new DalConfigurationSection();
        public AuthConfigurationSection AUTH { get; set; } = new AuthConfigurationSection();
        public CacheConfigurationSection CACHE { get; set; } = new CacheConfigurationSection();
        public MessageQueueConfigurationSection MQ { get; set; } = new MessageQueueConfigurationSection();
        public ApmConfigurationSection APM { get; set; } = new ApmConfigurationSection();
        public LocalConfiguration_Option LocalCfg { get; set; } = new LocalConfiguration_Option();
    }

    public class LocationConfigurationSection : Measurement_Option
    {
        public bool? AsConfigServerEnabled { get; set; }
        public bool? ServiceDiscoveryEnabled { get; set; }
        public string ServiceDiscoveryHostUrl { get; set; }
        public bool? ListenLocationEventEnabled { get; set; }
        public bool? BroadLocationEventEnabled { get; set; }
        public bool? ValidateIp { get; set; }
        public string CIDR { get; set; }
        public string ServiceName { get; set; }
        public string Tags { get; set; }
        public AwsSQS_Option SQS { get; set; } = new AwsSQS_Option();
    }

    public class ApiConfigurationSection : Measurement_Option
    {
        public string BaseUrl { get; set; }
        public string BearToken { get; set; }
    }
    public class DalConfigurationSection : Measurement_Option { }
    public class MessageQueueConfigurationSection : Measurement_Option { }
    public class NotificationConfigurationSection : Measurement_Option
    {
        public Dictionary<string, string> Channels { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
    public class ApmConfigurationSection : Measurement_Option
    {
        public int? MaxMessageCount { get; set; }
        public int? SendFrequency { get; set; }
        public string InfluxHostUri { get; set; }
        public string InfluxDefaultDB { get; set; }
        public string InfluxDbUser { get; set; }
        public string InfluxDbPwd { get; set; }
        public bool? IsSilent { get; set; }
    }

    public class CacheConfigurationSection : Measurement_Option
    {
        public string DefaultProvider { get; set; }
    }

    public class AuthConfigurationSection : Measurement_Option
    {
        public string DefaultProvider { get; set; }
    }
}
