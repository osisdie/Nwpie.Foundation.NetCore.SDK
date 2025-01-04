namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class ConfigItem
    {
        public ConfigItem() { }

        public ConfigItem(string configKey)
        {
            ConfigKey = configKey;
        }

        public ConfigItem(string configKey, string version)
        {
            ConfigKey = configKey;
            Version = version;
        }

        public string ConfigKey { get; set; }
        public string Version { get; set; }
    }
}
