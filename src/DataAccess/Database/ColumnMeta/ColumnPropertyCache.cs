using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal static class ColumnPropertyCache
    {
        public static ColumnProperty GetColumnProperty(string tableAlias, string columnName)
        {
            var columnPropertyCollection = ensureColumnPropertyRetrieved(tableAlias);
            if (columnPropertyCollection.Contains(columnName))
            {
                return columnPropertyCollection[columnName];
            }

            return null;
        }

        public static List<ColumnProperty> getPrimaryKeyColumnProperties(string tableAlias)
        {
            var columnPropertyCollection = ensureColumnPropertyRetrieved(tableAlias);
            return columnPropertyCollection.PrimaryKeyColumnProperties;
        }

        public static ColumnPropertyCollection ensureColumnPropertyRetrieved(string tableAlias)
        {
            return cache.GetOrAdd(tableAlias, (name) =>
            {
                var lazy = new Lazy<ColumnPropertyCollection>(delegate
                {
                    var provider = ConfigManager.Instance.GetProviderByDataBaseName(tableAlias.Split(new char[] { '.' })[0]);
                    var columnPropertyCollection = new ColumnPropertyCollection();
                var columnMetaProviderBase = provider switch
                {
                    DataSourceEnum.MySQL => (ColumnMetaProviderBase)new MySqlColumnMetaProvider(),
                    DataSourceEnum.SqlServer => (ColumnMetaProviderBase)new MsSqlColumnMetaProvider(),
                    _ => throw new NotSupportedException(provider.ToString())
                };

                    var columnMetas = columnMetaProviderBase.GetTableColumnMetas(tableAlias);

                    foreach (var current in columnMetas)
                    {
                        var columnProperty = new ColumnProperty
                        {
                            ColumnName = current.ColumnName,
                            IsNullable = current.AllowDBNull,
                            IsPrimaryKey = current.IsPrimaryKey,
                            Precision = current.Precision,
                            Scale = current.Scale,
                            Size = current.MaxLength,
                            DbType = current.DbType,
                            IsIdentity = current.IsIdentity
                        };

                        columnPropertyCollection.Add(columnProperty);
                    }

                    return columnPropertyCollection;
                });

                return lazy.Value;
            });
        }

        private static readonly ConcurrentDictionary<string, ColumnPropertyCollection> cache = new ConcurrentDictionary<string, ColumnPropertyCollection>(StringComparer.OrdinalIgnoreCase);
    }
}
