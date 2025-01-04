using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.DataAccess.Database.Configuration;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class FlexibleCommandBuilder : BaseCommandBuilder
    {
        public FlexibleCommandBuilder(string cmdName)
        {
            CommandConfigInfo = ConfigManager.Instance.GetCommandConfigInfoByName(cmdName);
            FillParameterCollection(CommandConfigInfo);
        }

        public FlexibleCommandBuilder(string cmdText, string connStr, int cmdTimeout, CommandType cmdType, DataSourceEnum provider = DataSourceEnum.UnSet)
        {
            if (DataSourceEnum.UnSet == provider)
                throw new ArgumentException(nameof(provider));

            CommandConfigInfo.CommandName = string.Empty;
            CommandConfigInfo.CommandText = cmdText;
            CommandConfigInfo.CommandType = cmdType;
            CommandConfigInfo.CommandTimeout = cmdTimeout;
            CommandConfigInfo.ConnectionString = connStr;
            CommandConfigInfo.Provider = provider;
            FillParameterCollection(CommandConfigInfo);
        }

        public override ICommand Build()
        {
            ICommand command = new BaseCommand()
            {
                Parameters = Parameters,
                Provider = CommandConfigInfo.Provider,
                CommandType = CommandConfigInfo.CommandType,
                ConnectionString = CommandConfigInfo.ConnectionString,
                CommandTimeout = CommandConfigInfo.CommandTimeout
            };

            command.CommandText = CombinParameters(CommandConfigInfo.CommandText);
            return command;
        }

        protected void FillParameterCollection(CommandConfigInfo cmdConfigInfo)
        {
            ParameterCollection = new Dictionary<string, FlexibleParameter>(StringComparer.OrdinalIgnoreCase);
            if (cmdConfigInfo?.Parameters?.Count() > 0)
            {
                foreach (var item in cmdConfigInfo.Parameters)
                {
                    var paramName = item.Value.Name.Trim();
                    if (ParameterCollection.Keys.Contains(paramName))
                    {
                        throw new ArgumentException($"Duplicate parameter name {item.Value.Name}. ");
                    }

                    var flexibleParameter = new FlexibleParameter()
                    {
                        Enable = false,
                        IsDiy = true,
                        Name = paramName,
                        Value = item.Value.Sql
                    };

                    ParameterCollection.Add(paramName, flexibleParameter);
                }
            }
        }

        protected string CombinParameters(string cmdText)
        {
            var sb = new StringBuilder(cmdText);
            foreach (var item in ParameterCollection)
            {
                if (item.Value.Enable)
                {
                    sb.Replace("/*{" + item.Value.Name + "}*/", item.Value.Value.ToString());
                    //cmdText = Regex.Replace(cmdText, string.Format(SpecificDiyParameterFormatter, item.Value.Name), item.Value.Value.ToString(), RegexOptions.IgnoreCase);
                }
            }

            //return cmdText;
            return sb.ToString();
        }

        protected readonly Regex DiyParameterSectionPattern = new Regex(@"/\*[\s]*{[\s]*[\w]+[\s]*}[\s]*\*/", RegexOptions.IgnoreCase);
        protected readonly string SpecificDiyParameterFormatter = "/\\*[\\s]*{{[\\s]*{0}[\\s]*}}[\\s]*\\*/";
    }
}
