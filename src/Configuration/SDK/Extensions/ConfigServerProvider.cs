using System;
using System.Collections.Generic;
using System.IO;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Configuration.SDK.Providers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Configuration.SDK.Extensions
{
    public class ConfigServerProvider<TConfig> : ConfigurationProvider
    {
        public ConfigServerProvider(ConfigServerSource<TConfig> source)
        {
            Source = source
                ?? throw new ArgumentNullException(nameof(ConfigServerSource<TConfig>));
        }

        public override void Load()
        {
            if (false == ConfigServerExtension.IsFinished)
            {
                lock (m_Lock)
                {
                    if (false == ConfigServerExtension.IsFinished)
                    {
                        var ConfigItems = new List<ConfigItem>();
                        foreach (var configKey in ConfigServerExtension.ConfigKeyList())
                        {
                            if (false == ConfigServerExtension.IsRetrieved(configKey))
                            {
                                ConfigItems.Add(new ConfigItem
                                {
                                    ConfigKey = configKey,
                                    Version = ConfigConst.LatestVersion
                                });
                            }
                        }

                        if (ServiceContext.IsLocalConfigFirst())
                        {
                            TryLocalLoad(ConfigItems);
                            ConfigServerExtension.IsFinished = true;
                        }
                        else if (ServiceContext.IsLocationAsConfigServer())
                        {
                            m_HostUrl = ServiceContext.LocationServiceUrl.ResolveGetActionUrl(LocationConst.HttpPathToConfigContractControllerName);
                            TryLoadFromConfigServer(ConfigItems, m_HostUrl);
                        }

                        if (true != ConfigServerExtension.IsFinished)
                        {
                            m_HostUrl = ServiceContext.ConfigServiceUrl.ResolveGetActionUrl();
                            TryLoadFromConfigServer(ConfigItems, m_HostUrl);
                        }
                    }
                }
            }

            if (ConfigServerExtension.IsRetrieved(Source.ConfigKey))
            {
                Data = ConfigServerParser.Parse<TConfig>(Source.ConfigKey,
                    ConfigServerExtension.GetValue(Source.ConfigKey)
                );
            }
        }

        void TryLocalLoad(List<ConfigItem> ConfigItems)
        {
            if (true != ConfigItems?.Count > 0)
            {
                return;
            }

            try
            {
                var path = Path.Combine(ServiceContext.Config.LocalCfg.BasePath ?? AppDomain.CurrentDomain.BaseDirectory, ServiceContext.Config.LocalCfg.LocalConfigFilePath);
                var text = File.ReadAllText(path);
                var dict = JsonConvert.DeserializeObject<FeatureToggle_Option>(text);
                foreach (var o in dict?.ConfigMap)
                {
                    ConfigServerExtension.Retrieved(o.Key, o.Value?.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Critical] {ex}");
            }
        }

        void TryLoadFromConfigServer(List<ConfigItem> ConfigItems, string getUrl)
        {
            if (true != ConfigItems?.Count > 0)
            {
                return;
            }

            IConfigClient configClient = null;
            try
            {
                configClient = new DefaultRemoteConfigClient(new DefaultSerializer());
                if (getUrl.HasValue())
                {
                    configClient.GetUrl = getUrl;
                }

                var taskGet = configClient.GetLatest(ConfigItems).ConfigureAwait(false).GetAwaiter().GetResult();
                if (true != taskGet?.Data?.Count > 0)
                {
                    throw new Exception($"Failed to read config from config server(={m_HostUrl}. msg={taskGet?.ErrMsg}. ");
                }

                foreach (var key in taskGet.Data.Keys)
                {
                    ConfigServerExtension.Retrieved(key, taskGet.Data[key]);
                }

                ConfigServerExtension.IsFinished = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Critical] {ex}");
            }
            finally
            {
                configClient?.Dispose();
            }
        }

        public ConfigServerSource<TConfig> Source { get; private set; }

        private static readonly object m_Lock = new object();
        private static readonly ISerializer m_Serializer = new DefaultSerializer();
        private static string m_HostUrl;
    }
}
