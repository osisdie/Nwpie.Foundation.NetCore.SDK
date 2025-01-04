using System;
using System.Linq;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.DataAccess.Database.Configuration;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class ConfigManager : SingleCObject<ConfigManager>
    {
        protected override void InitialInConstructor()
        {
            m_Config = new DALConfigReader().GetDALConfig();
        }

        public string GetConnectionStringByConnectionStrigName(string connStrName)
        {
            var connectionString = string.Empty;
            foreach (var items in m_Config.DatabaseSets.Values)
            {
                foreach (var item in items)
                {
                    if (item.ConnectionStringName == connStrName)
                    {
                        connectionString = item.ConnectionString;
                        break;
                    }
                }
            }

            return connectionString;
        }

        public string GetConnectionStringByDatabaseName(string dbName)
        {
            if (false == m_Config.DatabaseSets.Keys.Contains(dbName))
            {
                return string.Empty;
            }

            var connectionString = string.Empty;
            var dbSets = m_Config.DatabaseSets[dbName];

            if (1 == dbSets.Count)
            {
                connectionString = dbSets[0].ConnectionString;
            }
            else if (dbSets.Count > 1)
            {
                var defaultSets = dbSets.Where(o => o.IsDefault).ToList();
                if (1 != defaultSets.Count)
                {
                    throw new Exception($"Please specify a isDefault attribute for {dbName} DB in DbConfigs/*.configfile ");
                }

                connectionString = defaultSets.FirstOrDefault().ConnectionString;
            }

            return connectionString;
        }

        public string GetConnectionStringByCommandName(string cmdName)
        {
            if (string.IsNullOrWhiteSpace(cmdName))
            {
                return string.Empty;
            }

            var connStrName = m_Config.CommandConfigInfos[cmdName].ConnectionStringName;
            return GetConnectionStringByConnectionStrigName(connStrName);
        }

        public CommandConfigInfo GetCommandConfigInfoByName(string cmdName)
        {
            return m_Config.CommandConfigInfos[cmdName];
        }

        public DataSourceEnum GetProviderByDataBaseName(string dbName)
        {
            var provider = DataSourceEnum.UnSet;
            if (false == m_Config.DatabaseSets.Keys.Any(o => o.Contains(dbName, StringComparison.OrdinalIgnoreCase)))
            {
                return provider;
            }

            var dbSets = m_Config.DatabaseSets[dbName];
            if (1 == dbSets.Count)
            {
                provider = dbSets[0].Provider;
            }
            else if (dbSets.Count > 1)
            {
                var defaultSets = dbSets
                    .Where(o => o.IsDefault)
                    .ToList();

                if (1 != defaultSets.Count)
                {
                    throw new Exception($"Please specify a isDefault attribute for {dbName} DB in DbConfigs/*.configfile ");
                }

                provider = defaultSets
                    .FirstOrDefault()
                    .Provider;
            }

            return provider;
        }

        public override void Dispose() { }

        protected DALConfig m_Config;
    }
}
