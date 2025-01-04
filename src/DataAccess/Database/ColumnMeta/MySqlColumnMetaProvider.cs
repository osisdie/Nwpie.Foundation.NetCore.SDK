using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Nwpie.Foundation.DataAccess.Database
{
    class MySqlColumnMetaProvider : ColumnMetaProviderBase
    {
        protected override DataTable DoGetSchemaTable(string dbName, string tableName)
        {
            DataTable schemaTable;
            using (var mySqlCommand = new MySqlCommand(string.Format("SELECT * FROM {0} LIMIT 0,0", tableName)))
            {
                try
                {
                    mySqlCommand.Connection = new MySqlConnection(ConfigManager.Instance.GetConnectionStringByDatabaseName(dbName));
                    using (mySqlCommand.Connection)
                    {
                        if (ConnectionState.Open != mySqlCommand.Connection.State)
                        {
                            mySqlCommand.Connection.Open();
                        }

                        using (IDataReader dataReader = mySqlCommand.ExecuteReader())
                        {
                            schemaTable = dataReader.GetSchemaTable();
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (mySqlCommand.Connection.State == ConnectionState.Open)
                    {
                        mySqlCommand.Connection.Close();
                    }
                }
            }

            return schemaTable;
        }

        protected override ColumnMetaInfoCollection DoGetTableColumnMetas(string dbName, string tableName)
        {
            var columnMetaInfoCollection = new ColumnMetaInfoCollection();
            var dataTable = DoGetSchemaTable(dbName, tableName);

            if (null != dataTable)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var columnMetaInfo = new ColumnMetaInfo
                    {
                        ColumnName = dataRow["ColumnName"].ToString(),
                        ColumnOrdinal = Convert.ToInt32(dataRow["ColumnOrdinal"]),
                        AllowDBNull = Convert.ToBoolean(dataRow["AllowDBNull"]),
                        MaxLength = Convert.ToInt32(dataRow["ColumnSize"]),
                        IsIdentity = (null != dataRow["IsAutoIncrement"] && 1 == Convert.ToInt32(dataRow["IsAutoIncrement"]))
                    };

                    var dataType = "";
                    if (dataTable.Columns.Contains("DataType") &&
                        null != dataRow["DataType"])
                    {
                        var array = dataRow["DataType"].ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (2 == array.Length)
                        {
                            dataType = array[1];
                        }
                    }

                    columnMetaInfo.DbType = ToDbType(dataType, out var flag, out var defaultPrecision, out var defaultScale);
                    columnMetaInfo.IsPrimaryKey = Convert.ToBoolean(dataRow["IsKey"]);
                    if (flag)
                    {
                        columnMetaInfo.Precision = defaultPrecision;
                        columnMetaInfo.Scale = defaultScale;
                    }
                    else
                    {
                        var numericPrecision = Convert.ToByte(dataRow["NumericPrecision"]);
                        columnMetaInfo.Precision = ((numericPrecision == 255) ? defaultPrecision : numericPrecision);

                        var numericScale = Convert.ToByte(dataRow["NumericScale"]);
                        columnMetaInfo.Scale = ((numericScale == 255) ? defaultScale : numericScale);
                    }

                    if (false == columnMetaInfoCollection.Contains(columnMetaInfo.ColumnName))
                    {
                        columnMetaInfoCollection.Add(columnMetaInfo);
                    }
                }
            }

            return columnMetaInfoCollection;
        }

        protected DbType ToDbType(string dataType, out bool fixedColumn, out byte defaultPrecision, out byte defaultScale)
        {
            defaultPrecision = 0;
            defaultScale = 0;
            fixedColumn = true;

            if (false == Enum.TryParse<MySqlDbType>(dataType, true, out var mySqlDbType))
            {
                var flag = false;
                if (dataType == "SByte")
                {
                    mySqlDbType = MySqlDbType.Byte;
                    flag = true;
                }

                if (dataType == "Single")
                {
                    mySqlDbType = MySqlDbType.Float;
                    flag = true;
                }

                if (false == flag)
                {
                    throw new ArgumentException($"Invalid column type:{dataType}. ");
                }
            }

            var mySqlParameter = new MySqlParameter
            {
                MySqlDbType = mySqlDbType
            };

            switch (mySqlDbType)
            {
                case MySqlDbType.Decimal:
                    defaultPrecision = 18;
                    defaultScale = 0;
                    fixedColumn = false;
                    break;

                case MySqlDbType.Byte:
                    defaultPrecision = 3;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Int16:
                    defaultPrecision = 5;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Int32:
                    defaultPrecision = 10;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Float:
                    defaultPrecision = 53;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Int64:
                    defaultPrecision = 19;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Date:
                    defaultPrecision = 10;
                    defaultScale = 0;
                    break;

                case MySqlDbType.Time:
                    defaultPrecision = 16;
                    defaultScale = 7;
                    fixedColumn = false;
                    break;

                case MySqlDbType.DateTime:
                    defaultPrecision = 23;
                    defaultScale = 3;
                    break;

                case MySqlDbType.Bit:
                    defaultPrecision = 1;
                    defaultScale = 0;
                    break;

                default:
                    defaultPrecision = 0;
                    defaultScale = 0;
                    break;
            }

            return mySqlParameter.DbType;
        }
    }
}
