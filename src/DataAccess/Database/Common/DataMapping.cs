using System;
using System.Collections.Generic;
using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    /// <summary>
    /// A class for mapping data types
    /// </summary>
    internal static class DataMapping
    {
        static DataMapping()
        {
            TypeToDbType = new Dictionary<Type, DbType>
            {
                {typeof(byte[]), DbType.Binary},
                {typeof(bool), DbType.Boolean},
                {typeof(byte), DbType.Byte},
                {typeof(DateTime), DbType.DateTime2}, // be careful microseconds
                {typeof(DateTimeOffset), DbType.DateTimeOffset},
                {typeof(decimal), DbType.Decimal},
                {typeof(double), DbType.Double},
                {typeof(Guid), DbType.Guid},
                {typeof(short), DbType.Int16},
                {typeof(int), DbType.Int32},
                {typeof(long), DbType.Int64},
                {typeof(sbyte), DbType.SByte},
                {typeof(float), DbType.Single},
                {typeof(string), DbType.String},
                {typeof(TimeSpan), DbType.Time},
                {typeof(ushort), DbType.UInt16},
                {typeof(uint), DbType.UInt32},
                {typeof(ulong), DbType.UInt64}
            };

            DbTypeToType = new Dictionary<DbType, Type>
            {
                {DbType.AnsiString, typeof(string)},
                {DbType.AnsiStringFixedLength, typeof(string)},
                {DbType.Binary, typeof(byte[])},
                {DbType.Boolean, typeof(bool)},
                {DbType.Byte, typeof(byte)},
                {DbType.Currency, typeof(decimal)},
                {DbType.Date, typeof(DateTime)},
                {DbType.DateTime, typeof(DateTime)}, // be careful microseconds
                {DbType.DateTime2, typeof(DateTime)}, // be careful microseconds
                {DbType.Decimal, typeof(decimal)},
                {DbType.Double, typeof(double)},
                {DbType.Guid, typeof(Guid)},
                {DbType.Int16, typeof(short)},
                {DbType.Int32, typeof(int)},
                {DbType.Int64, typeof(long)},
                {DbType.Object, typeof(object)},
                {DbType.SByte, typeof(sbyte)},
                {DbType.Single, typeof(float)},
                {DbType.String, typeof(string)},
                {DbType.StringFixedLength, typeof(string)},
                {DbType.Time, typeof(TimeSpan)},
                {DbType.UInt16, typeof(ushort)},
                {DbType.UInt32, typeof(uint)},
                {DbType.UInt64, typeof(ulong)},
                {DbType.VarNumeric, typeof(decimal)},
                {DbType.DateTimeOffset, typeof(DateTimeOffset)},
            };
        }

        /// <summary>
        /// Converts system <see cref="Type"/> to a <see cref="DbType"/>.
        /// </summary>
        /// <param name="type">The system type to convert.</param>
        /// <returns>A <see cref="DbType"/> for the system <see cref="Type"/>.</returns>
        public static DbType ToDbType(this Type type)
        {
            if (TypeToDbType.TryGetValue(type, out var dbType))
            {
                return dbType;
            }

            return DbType.Object;
        }

        /// <summary>
        /// Converts <see cref="DbType"/> to a system <see cref="Type"/>.
        /// </summary>
        /// <param name="dbType">The DbType to convert.</param>
        /// <returns>A system <see cref="Type"/> for the <see cref="DbType"/>.</returns>
        public static Type ToType(this DbType dbType)
        {
            if (DbTypeToType.TryGetValue(dbType, out var type))
            {
                return type;
            }

            return typeof(object);
        }

        /// <summary>
        /// Gets the underlying type dealing with <see cref="T:Nullable`1"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns a type dealing with <see cref="T:Nullable`1"/>.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type), $"from {typeof(DataMapping).Name}");
            }

            var isNullable = type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
            {
                return Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        private static readonly Dictionary<Type, DbType> TypeToDbType;
        private static readonly Dictionary<DbType, Type> DbTypeToType;
    }
}
