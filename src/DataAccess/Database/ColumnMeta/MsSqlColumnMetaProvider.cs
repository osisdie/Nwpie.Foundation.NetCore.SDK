using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class MsSqlColumnMetaProvider : ColumnMetaProviderBase
    {
        protected override DataTable DoGetSchemaTable(string dbName, string tableName)
        {
            DataTable schemaTable;
            using var sqlCommand = new SqlCommand($"SELECT TOP 0 * FROM {tableName} WITH(NOLOCK)");

            try
            {
                var connectionString = ConfigManager.Instance.GetConnectionStringByDatabaseName(dbName);
                using (sqlCommand.Connection = new SqlConnection(connectionString))
                {
                    sqlCommand.Connection.Open();

                    using var reader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
                    schemaTable = reader.GetSchemaTable();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sqlCommand.Connection != null
                    && ConnectionState.Closed != sqlCommand.Connection.State)
                {
                    sqlCommand.Connection.Close();
                }
            }

            return schemaTable;
        }

        protected override ColumnMetaInfoCollection DoGetTableColumnMetas(string dbName, string tableName)
        {
            var res = new ColumnMetaInfoCollection();

            var dataTable = DoGetSchemaTable(dbName, tableName);
            if (dataTable is null)
            {
                return res;
            }

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var columnMetaInfo = new ColumnMetaInfo
                {
                    ColumnName = dataRow["ColumnName"].ToString(),
                    ColumnOrdinal = Convert.ToInt32(dataRow["ColumnOrdinal"]),
                    AllowDBNull = Convert.ToBoolean(dataRow["AllowDBNull"]),
                    MaxLength = Convert.ToInt32(dataRow["ColumnSize"]),
                    IsIdentity = dataRow["IsIdentity"] != null && Convert.ToInt32(dataRow["IsIdentity"]) == 1
                };

                columnMetaInfo.DbType = ToDbType(dataRow["DataTypeName"].ToString(), out var flag, out var defaultPrecision, out var defaultScale);
                columnMetaInfo.IsPrimaryKey = dataRow.Field<bool?>("IsKey") ?? false;

                if (flag)
                {
                    columnMetaInfo.Precision = defaultPrecision;
                    columnMetaInfo.Scale = defaultScale;
                }
                else
                {
                    var numericPrecision = Convert.ToByte(dataRow["NumericPrecision"]);
                    columnMetaInfo.Precision = (numericPrecision == 255) ? defaultPrecision : numericPrecision;

                    var numericScale = Convert.ToByte(dataRow["NumericScale"]);
                    columnMetaInfo.Scale = (numericScale == 255) ? defaultScale : numericScale;
                }

                if (!res.Contains(columnMetaInfo.ColumnName))
                {
                    res.Add(columnMetaInfo);
                }
            }

            return res;
        }

        protected DbType ToDbType(string dataType, out bool fixedColumn, out byte defaultPrecision, out byte defaultScale)
        {
            defaultPrecision = 0;
            defaultScale = 0;
            fixedColumn = true;
            if (!Enum.TryParse<SqlDbType>(dataType, ignoreCase: true, out var sqlDbType))
                throw new ArgumentException($"Invalid column type:{dataType}. ");

            var sqlParameter = new SqlParameter
            {
                SqlDbType = sqlDbType
            };

            var sqlDbType2 = sqlDbType;
            switch (sqlDbType2)
            {
                case SqlDbType.BigInt:
                    defaultPrecision = 19;
                    defaultScale = 0;
                    break;

                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.Image:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Text:
                case SqlDbType.Timestamp:
                    break;

                case SqlDbType.Bit:
                    defaultPrecision = 1;
                    defaultScale = 0;
                    break;

                case SqlDbType.DateTime:
                    defaultPrecision = 23;
                    defaultScale = 3;
                    break;

                case SqlDbType.Decimal:
                    defaultPrecision = 18;
                    defaultScale = 0;
                    fixedColumn = false;
                    break;

                case SqlDbType.Float:
                    defaultPrecision = 53;
                    defaultScale = 0;
                    break;
                case SqlDbType.Int:
                    defaultPrecision = 10;
                    defaultScale = 0;
                    break;

                case SqlDbType.Money:
                    defaultPrecision = 19;
                    defaultScale = 4;
                    break;

                case SqlDbType.Real:
                    defaultPrecision = 24;
                    defaultScale = 0;
                    break;

                case SqlDbType.SmallDateTime:
                    defaultPrecision = 16;
                    defaultScale = 0;
                    break;

                case SqlDbType.SmallInt:
                    defaultPrecision = 5;
                    defaultScale = 0;
                    break;

                case SqlDbType.SmallMoney:
                    defaultPrecision = 10;
                    defaultScale = 4;
                    break;

                case SqlDbType.TinyInt:
                    defaultPrecision = 3;
                    defaultScale = 0;
                    break;

                default:
                    switch (sqlDbType2)
                    {
                        case SqlDbType.Date:
                            defaultPrecision = 10;
                            defaultScale = 0;
                            break;

                        case SqlDbType.Time:
                            defaultPrecision = 16;
                            defaultScale = 7;
                            fixedColumn = false;
                            break;

                        case SqlDbType.DateTime2:
                            defaultPrecision = 27;
                            defaultScale = 7;
                            fixedColumn = false;
                            break;

                        case SqlDbType.DateTimeOffset:
                            defaultPrecision = 34;
                            defaultScale = 7;
                            break;
                    }
                    break;
            }

            return sqlParameter.DbType;
        }
    }
}
