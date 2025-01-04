using System;
using Microsoft.Extensions.Configuration;

namespace Nwpie.Foundation.Configuration.SDK.Extensions
{
    public class ConfigServerSource<TConfig> : IConfigurationSource
    {
        public ConfigServerSource(string configKey)
        {
            ConfigKey = configKey
                ?? throw new ArgumentNullException(nameof(configKey));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new ConfigServerProvider<TConfig>(this);

        public string ConfigKey { get; private set; }
    }
}
