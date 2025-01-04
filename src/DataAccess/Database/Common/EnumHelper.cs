using System;
using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    public static class EnumHelper
    {
        public static ParameterDirection ConvertToDirection(string direction)
        {
            switch (direction.Trim().ToLower())
            {
                case "input":
                    return ParameterDirection.Input;
                case "inputoutput":
                    return ParameterDirection.InputOutput;
                case "output":
                    return ParameterDirection.Output;
                case "returnvalue":
                    return ParameterDirection.ReturnValue;
                default:
                    throw new InvalidCastException($"Cannot convert {direction} to ParameterDirection. ");
            }
        }

        public static CommandType ConvertToCommandType(string commandType)
        {
            switch (commandType.Trim().ToLower())
            {
                case "text":
                    return CommandType.Text;
                case "storedprocedure":
                    return CommandType.StoredProcedure;
                case "tabledirect":
                    return CommandType.TableDirect;
                default:
                    throw new InvalidCastException($"Cannot convert {commandType} to CommandType.");
            }
        }

        public static DbType ConvertToDbType(string dbType)
        {
            switch (dbType.Trim().ToLower())
            {
                case "ansistring":
                    return DbType.AnsiString;
                case "binary":
                    return DbType.Binary;
                case "byte":
                    return DbType.Byte;
                case "boolean":
                    return DbType.Boolean;
                case "currency":
                    return DbType.Currency;
                case "date":
                    return DbType.Date;
                case "datetime":
                    return DbType.DateTime; // be careful microseconds
                case "datetime2":
                    return DbType.DateTime2; // be careful microseconds
                case "decimal":
                    return DbType.Decimal;
                case "double":
                    return DbType.Double;
                case "guid":
                    return DbType.Guid;
                case "int16":
                    return DbType.Int16;
                case "int32":
                    return DbType.Int32;
                case "int64":
                    return DbType.Int64;
                case "object":
                    return DbType.Object;
                case "sbyte":
                    return DbType.SByte;
                case "single":
                    return DbType.Single;
                case "string":
                    return DbType.String;
                case "time":
                    return DbType.Time;
                case "uint16":
                    return DbType.UInt16;
                case "uint32":
                    return DbType.UInt32;
                case "uint64":
                    return DbType.UInt64;
                case "varnumeric":
                    return DbType.VarNumeric;
                case "ansistringfixedlength":
                    return DbType.AnsiStringFixedLength;
                case "stringfixedlength":
                    return DbType.StringFixedLength;
                case "xml":
                    return DbType.Xml;
                case "datetimeoffset":
                    return DbType.DateTimeOffset;
                default:
                    throw new InvalidCastException($"Cannot convert {dbType} to DbType. ");
            }
        }

        public static SqlDbType ConvertToSqlDbType(string sqlDbType)
        {
            switch (sqlDbType.Trim().ToLower())
            {
                case "bigint":
                    return SqlDbType.BigInt;
                case "binary":
                    return SqlDbType.Binary;
                case "bit":
                    return SqlDbType.Bit;
                case "char":
                    return SqlDbType.Char;
                case "date":
                    return SqlDbType.Date;
                case "datetime":
                    return SqlDbType.DateTime; // truncate microseconds
                case "datetime2":
                    return SqlDbType.DateTime2; // truncate microseconds
                case "decimal":
                    return SqlDbType.Decimal;
                case "float":
                    return SqlDbType.Float;
                case "image":
                    return SqlDbType.Image;
                case "int":
                    return SqlDbType.Int;
                case "money":
                    return SqlDbType.Money;
                case "nchar":
                    return SqlDbType.NChar;
                case "ntext":
                    return SqlDbType.NText;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                case "real":
                    return SqlDbType.Real;
                case "uniqueidentifier":
                    return SqlDbType.UniqueIdentifier;
                case "smalldatetime":
                    return SqlDbType.SmallDateTime;
                case "smallint":
                    return SqlDbType.SmallInt;
                case "smallmoney":
                    return SqlDbType.SmallMoney;
                case "text":
                    return SqlDbType.Text;
                case "timestamp":
                    return SqlDbType.Timestamp;
                case "tinyint":
                    return SqlDbType.TinyInt;
                case "varbinary":
                    return SqlDbType.VarBinary;
                case "varchar":
                    return SqlDbType.VarChar;
                case "variant":
                    return SqlDbType.Variant;
                case "xml":
                    return SqlDbType.Xml;
                case "udt":
                    return SqlDbType.Udt;
                case "structured":
                    return SqlDbType.Structured;
                case "time":
                    return SqlDbType.Time;
                case "datetimeoffset":
                    return SqlDbType.DateTimeOffset;
                default:
                    throw new InvalidCastException($"Cannot convert {sqlDbType} to SqlDbType. ");
            }
        }

        public static string ConvertEntityOperationEnumToString(OperationEnum operation)
        {
            var result = string.Empty;
            switch (operation)
            {
                case OperationEnum.Insert:
                    result = "Insert";
                    break;
                case OperationEnum.Delete:
                    result = "Delete";
                    break;
                case OperationEnum.Update:
                    result = "Update";
                    break;
            }

            return result;
        }

        public static Type ConvertSqlDbTypeToType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(object);
                case SqlDbType.Bit:
                    return typeof(bool);
                case SqlDbType.Char:
                    return typeof(string);
                case SqlDbType.Date:
                    return typeof(DateTime);
                case SqlDbType.DateTime:
                    return typeof(DateTime); // be careful microseconds
                case SqlDbType.DateTime2:
                    return typeof(DateTime); // be careful microseconds
                case SqlDbType.Decimal:
                    return typeof(decimal);
                case SqlDbType.Float:
                    return typeof(double);
                case SqlDbType.Image:
                    return typeof(object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(decimal);
                case SqlDbType.NChar:
                    return typeof(string);
                case SqlDbType.NText:
                    return typeof(string);
                case SqlDbType.NVarChar:
                    return typeof(string);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(decimal);
                case SqlDbType.Text:
                    return typeof(string);
                case SqlDbType.Timestamp:
                    return typeof(object);
                case SqlDbType.TinyInt:
                    return typeof(byte);
                case SqlDbType.Udt:
                    return typeof(object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(object);
                case SqlDbType.VarBinary:
                    return typeof(object);
                case SqlDbType.VarChar:
                    return typeof(string);
                case SqlDbType.Variant:
                    return typeof(object);
                case SqlDbType.Xml:
                    return typeof(object);
                default:
                    return null;
            }
        }
    }
}
