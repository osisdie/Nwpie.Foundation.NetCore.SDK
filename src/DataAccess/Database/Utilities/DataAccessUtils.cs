using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nwpie.Foundation.Abstractions.DataAccess.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database.Utilities
{
    public static class DataAccessUtils
    {
        public static MySql.Data.MySqlClient.MySqlConnection GetMySqlConnection(string connectionString, bool open = true,
            bool convertZeroDatetime = ConvertZeroDatetime, bool allowZeroDatetime = AllowZeroDatetime)
        {
            var connectionStringBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString)
            {
                AllowZeroDateTime = allowZeroDatetime,
                ConvertZeroDateTime = convertZeroDatetime
            };

            var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionStringBuilder.ConnectionString);
            if (open)
            {
                conn.Open();
            }

            return conn;
        }

        public static string GetTableCacheKey(string dbName, string tableName, params object[] keys)
        {
            var cacheKey = $"{CachePrefix}{__}DB{__}{dbName}{__}{tableName}";
            if (keys?.Count() > 0)
            {
                var suffix = new StringBuilder();
                foreach (var key in keys)
                {
                    if (null == key || string.IsNullOrWhiteSpace(key.ToString()))
                    {
                        continue;
                    }

                    suffix.Append($"{__}{key.ToString().ToMD5()}");
                }

                cacheKey += $"{__}{suffix.ToString()}";
            }

            return cacheKey.ToLower();
        }

        public static InspectionData InspectSqlCommand(string sqlcommand, DynamicParameters parameters)
        {
            var result = new InspectionData()
            {
                OriginalSqlCommand = sqlcommand,
                ReplacedSqlCommand = sqlcommand,
            };

            if (string.IsNullOrWhiteSpace(sqlcommand) ||
                true != parameters?.ParameterNames?.Count() > 0)
            {
                return result;
            }

            foreach (var name in parameters?.ParameterNames)
            {
                var pValue = parameters.Get<dynamic>(name);
                result.OriginalParameters.Add(name, pValue);
            }

            var matches = Regex.Matches(sqlcommand, SqlParameterPattern);
            foreach (var name in parameters.ParameterNames)
            {
                var pValue = parameters.Get<dynamic>(name);
                var stringSymbol = (pValue is string | pValue is DateTime) ? "'" : string.Empty;
                result.ReplacedSqlCommand = result.ReplacedSqlCommand.Replace($"@{name}", $"{stringSymbol}{pValue}{stringSymbol}");
            }

            foreach (Match match in matches)
            {
                var p = match.Groups[SqlExtractParamGroup].Value;
                if (p.HasValue() &&
                    false == parameters.ParameterNames.Contains(p))
                {
                    result.MissingParameters.Add(p);
                }
            }

            return result;
        }

        public const string __ = CommonConst.Separator;
        public static string CachePrefix = ServiceContext.SdkEnv;
        public const string SqlExtractParamGroup = "parameter";
        public const string SqlReservedChars = @"[\s\&\=\~\|\^\/\.\-\+\*\(\)\!:;,><%'`""]+";
        public const string SqlParameterPattern = @"@(?<parameter>\w+)" + SqlReservedChars;
        public const bool ConvertZeroDatetime = true;
        public const bool AllowZeroDatetime = false;
    }
}
