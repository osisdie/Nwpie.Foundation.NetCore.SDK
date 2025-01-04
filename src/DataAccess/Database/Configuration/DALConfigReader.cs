using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Nwpie.Foundation.DataAccess.Database.Configuration
{
    public class DALConfigReader : CObject
    {
        static DALConfigReader()
        {
            m_Serializer = new DefaultSerializer();
        }

        public DALConfig GetDALConfig()
        {
            return LoadDALConfig();
        }

        protected DALConfig LoadDALConfig()
        {
            var dalConfig = new DALConfig();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigConst.DefaultDbConfigsFolder);
            var filePattern = $"{ConfigConst.DefaultDbConfigsFilePrefix}*.{ConfigConst.DefaultDbConfigsFileExtension}";
            var files = Directory.GetFiles(path, filePattern, SearchOption.AllDirectories);

            Logger.LogTrace(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.DALInitialization.GetDisplayName() },
                { SysLoggerKey.Path, path },
                { SysLoggerKey.FilePattern, filePattern },
                { SysLoggerKey.Files, files },
                { SysLoggerKey.Caller, GetType().Name },
            }.AddTraceData()));

            foreach (var file in files)
            {
                var xDoc = XDocument.Load(file);
                var rootElement = xDoc.Element("dalConfig");
                var databaseSets = rootElement.Elements("databaseSet");

                foreach (var databaseSet in databaseSets)
                {
                    var dbName = databaseSet.Attribute("databaseName")?.Value;
                    var providerName = databaseSet.Attribute("provider").Value.ToLower();
                    var provider = providerName switch
                    {
                        "mysql" => DataSourceEnum.MySQL,
                        "sqlserver" => DataSourceEnum.SqlServer,
                        _ => throw new NotSupportedException(providerName)
                    };

                    var connStrName = databaseSet.Attribute("connectionStringName")?.Value;
                    var isDefautlAttr = databaseSet.Attribute("isDefault");
                    var isDefault = null != isDefautlAttr
                        ? isDefautlAttr.Value.ToScalar<bool>()
                        : false;

                    string connStr = null;

                    try
                    {
                        connStr = ServiceContext.Configuration.GetValue<string>(connStrName);
                        if (string.IsNullOrWhiteSpace(connStr))
                        {
                            connStr = ServiceContext.Configuration.GetConnectionString(connStrName);
                        }

                        // Try read from web.config or app.config (legacy)
                        if (string.IsNullOrWhiteSpace(connStr))
                        {
                            connStr = ConfigurationManager.ConnectionStrings[connStrName]?.ConnectionString;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }

                    if (string.IsNullOrWhiteSpace(connStr))
                    {
                        Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { SysLoggerKey.Type, LoggingTypeEnum.DALException.GetDisplayName() },
                            { SysLoggerKey.Path, path },
                            { SysLoggerKey.FilePattern, filePattern },
                            { SysLoggerKey.Files, files },
                            { SysLoggerKey.Caller, GetType().Name },
                            { SysLoggerKey.Exception, $"Missing connection string '{connStrName}' in config file. " },
                        }.AddTraceData()));

                        throw new Exception($"Missing connection string '{connStrName}' in config file. ");
                    }

                    if (dalConfig.DatabaseSets.Keys.Contains(dbName, StringComparer.OrdinalIgnoreCase))
                    {
                        dalConfig.DatabaseSets[dbName].Add(
                            new DatabaseSet(dbName,
                                provider, connStrName, connStr, isDefault
                            )
                        );
                    }
                    else
                    {
                        dalConfig.DatabaseSets.Add(dbName,
                            new List<DatabaseSet>() {
                                new DatabaseSet(dbName, provider, connStrName, connStr, isDefault)
                            }
                        );
                    }

                    var dataCommands = databaseSet.Elements("dataCommand");
                    if (dataCommands?.Count() > 0)
                    {
                        foreach (var command in dataCommands)
                        {
                            var cmdName = command.Attribute("name").Value;
                            var cmdType = command.Attribute("commandType").Value;
                            var cmdText = command.Element("commandText").Value;
                            var cmdTimeOut = ConfigConst.DefaultDALTimeout;
                            var attr = command.Attribute("commandTimeOut");
                            if (null != attr)
                            {
                                int.TryParse(attr.Value, out cmdTimeOut);
                            }

                            if (dalConfig.CommandConfigInfos.ContainsKey(cmdName))
                            {
                                Logger.LogError($"Duplicate command(={cmdName}) are found. ");
                                throw new DuplicateNameException($"Duplicate command(={cmdName}) are found. ");
                            }

                            dalConfig.CommandConfigInfos.Add(cmdName,
                                new CommandConfigInfo(dbName,
                                    cmdName,
                                    EnumHelper.ConvertToCommandType(cmdType),
                                    cmdText,
                                    connStrName,
                                    connStr,
                                    cmdTimeOut,
                                    provider
                                )
                            );

                            var elParameter = command.Element("parameters");
                            if (null == elParameter)
                            {
                                continue;
                            }

                            var parameters = elParameter.Elements("add");
                            foreach (var parameter in parameters)
                            {
                                var paramName = parameter.Attribute("name").Value;
                                var paramSql = parameter.Value
                                    .Replace("\r", string.Empty)
                                    .Replace("\n", string.Empty)
                                    .Trim();

                                dalConfig.CommandConfigInfos[cmdName]
                                    ?.Parameters
                                    ?.Add(paramName, new Parameter(paramName, paramSql));
                            }
                        }
                    }
                }
            }

            return dalConfig;
        }

        private static readonly ISerializer m_Serializer;
    }

    public class DALConfig
    {
        public DALConfig()
        {
            DatabaseSets = new Dictionary<string, IList<DatabaseSet>>(StringComparer.OrdinalIgnoreCase);
            CommandConfigInfos = new Dictionary<string, CommandConfigInfo>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, IList<DatabaseSet>> DatabaseSets { get; private set; }
        public IDictionary<string, CommandConfigInfo> CommandConfigInfos { get; private set; }
    }

    public class DatabaseSet
    {
        public DatabaseSet(string dbName, DataSourceEnum provider, string connStrName, string connStr, bool isDefault)
        {
            DBName = dbName;
            Provider = provider;
            ConnectionStringName = connStrName;
            IsDefault = isDefault;
            ConnectionString = connStr;
        }

        public string DBName { get; private set; }
        public DataSourceEnum Provider { get; private set; }
        public string ConnectionStringName { get; private set; }
        public string ConnectionString { get; private set; }
        public bool IsDefault { get; private set; }
    }

    public class CommandConfigInfo
    {
        public CommandConfigInfo() { }

        public CommandConfigInfo(string dbName, string commandName, CommandType commandType, string commandText, string connStrName, string connStr, int commandTimeOut = ConfigConst.DefaultDALTimeout, DataSourceEnum provider = DataSourceEnum.UnSet)
        {
            if (DataSourceEnum.UnSet == provider)
                throw new ArgumentException(nameof(provider));

            DatabaseName = dbName;
            CommandName = commandName;
            CommandType = commandType;
            CommandText = commandText;
            CommandTimeout = commandTimeOut;
            ConnectionStringName = connStrName;
            ConnectionString = connStr;
            Provider = provider;
        }

        public string CommandName { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }
        public string ConnectionStringName { get; set; }
        public string ConnectionString { get; set; }
        public DataSourceEnum Provider { get; set; }
        public string DatabaseName { get; private set; }
        public IDictionary<string, Parameter> Parameters { get; private set; } = new Dictionary<string, Parameter>(StringComparer.OrdinalIgnoreCase);
    }

    public class Parameter
    {
        public Parameter(string name, string sql)
        {
            Name = name;
            Sql = sql;
        }

        public string Name { get; set; }
        public string Sql { get; set; }
    }
}
