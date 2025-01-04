using System;
using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    public abstract class ColumnMetaProviderBase : IColumnMetaProvider
    {
        public ColumnMetaInfoCollection GetTableColumnMetas(string tableAlias)
        {
            var array = tableAlias.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (null == array || array.Length < 2)
            {
                throw new ArgumentException($"Table alias not specified correctly. {tableAlias}");
            }

            return GetTableColumnMetas(array[0], array.Length == 3 ? string.Concat(array[1], ".", array[2]) : array[1]);
        }

        public ColumnMetaInfoCollection GetTableColumnMetas(string databaseName, string tableName) =>
            DoGetTableColumnMetas(databaseName, tableName);

        protected abstract ColumnMetaInfoCollection DoGetTableColumnMetas(string databaseName, string tableName);
        protected abstract DataTable DoGetSchemaTable(string databaseName, string tableName);
    }
}
