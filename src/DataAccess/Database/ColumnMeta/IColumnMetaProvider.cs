namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IColumnMetaProvider
    {
        ColumnMetaInfoCollection GetTableColumnMetas(string tableAlias);

        ColumnMetaInfoCollection GetTableColumnMetas(string databaseName, string tableName);
    }
}
