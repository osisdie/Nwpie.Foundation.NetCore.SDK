using System.Configuration;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Nwpie.Foundation.Configuration.SDK.Providers
{
    public class LegacyXmlConfigurationProvider : ConfigurationProvider, IConfigurationSource
    {
        public override void Load()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                Data.Add($"ConnectionStrings{__}{connectionString.Name}", connectionString.ConnectionString);
            }

            foreach (var settingKey in ConfigurationManager.AppSettings.AllKeys)
            {
                Data.Add(settingKey, ConfigurationManager.AppSettings[settingKey]);
                //Data.Add($"AppSettings{__}{settingKey}", ConfigurationManager.AppSettings[settingKey]);
            }
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;

        protected readonly string __ = ConfigurationPath.KeyDelimiter;
    }
}
