using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class BaseQueryCommandBuilder : IQueryCommandBuilder
    {
        public BaseQueryCommandBuilder()
        {
            TopRows = -1;
            IsExecuteCount = false;
            OrderBys = new List<OrderByItem>();
            Parameters = new DynamicParameters();
            Conditions = new QueryConditionCollection();
        }

        public ICommand Build()
        {
            var array = TableAlias.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (null == array || array.Length < 2)
                throw new ArgumentException($"Table alias ({TableAlias}) not specified correctly. ");

            var provider = ConfigManager.Instance.GetProviderByDataBaseName(array[0]);
            return provider switch
            {
                DataSourceEnum.MySQL => new MySqlQueryCommandBuilder(this).Build(),
                DataSourceEnum.SqlServer => new MsSqlQueryCommandBuilder(this).Build(),
                _ => throw new NotSupportedException(provider.ToString())
            };
        }

        public int TopRows { get; set; }
        public int SkipRows { get; set; }
        public bool IsDistinct { get; set; }
        public string TableAlias { get; set; }
        public bool IsExecuteCount { get; set; }
        public string[] ColumnNameFilters { get; set; }
        public List<OrderByItem> OrderBys { get; set; }
        public DynamicParameters Parameters { get; set; }
        public QueryConditionCollection Conditions { get; set; }
    }
}
