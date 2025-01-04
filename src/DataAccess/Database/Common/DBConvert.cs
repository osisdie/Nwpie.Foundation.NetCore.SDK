using System;
using System.Globalization;
using Nwpie.Foundation.Abstractions.Extensions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class DBConvert
    {
        #region Boolean

        public static bool ToBooleanInternal(object value, IFormatProvider provider)
        {
            var s = value as string;
            if (null != s && bool.TryParse(s, out var b))
            {
                return b;
            }

            if (null != (s = value.ToString()))
            {
                if (Int32.TryParse(s, out var i))
                {
                    return i != 0;
                }
            }

            return Convert.ToBoolean(value, provider);
        }

        public static bool? ToNullableBoolean(object value, IFormatProvider provider) =>
            ToNullable<bool>(value, provider, ToBooleanInternal);

        public static bool? ToNullableBoolean(object value) =>
            ToNullable<bool>(value, CultureInfo.CurrentCulture, ToBooleanInternal);

        public static bool ToBoolean(object value, IFormatProvider provider) =>
            To<bool>(value, provider, ToBooleanInternal);

        public static bool ToBoolean(object value) =>
            To<bool>(value, CultureInfo.CurrentCulture, ToBooleanInternal);

        #endregion

        #region SByte

        public static sbyte ToSByteInternal(object value, IFormatProvider provider) =>
            Convert.ToSByte(value, provider);

        public static sbyte? ToNullableSByte(object value, IFormatProvider provider) =>
            ToNullable<sbyte>(value, provider, ToSByteInternal);

        public static sbyte? ToNullableSByte(object value) =>
            ToNullable<sbyte>(value, CultureInfo.CurrentCulture, ToSByteInternal);

        public static sbyte ToSByte(object value, IFormatProvider provider) =>
            To<sbyte>(value, provider, ToSByteInternal);

        public static sbyte ToSByte(object value) =>
            To<sbyte>(value, CultureInfo.CurrentCulture, ToSByteInternal);

        #endregion

        #region Int16

        public static short ToInt16Internal(object value, IFormatProvider provider) =>
            Convert.ToInt16(value, provider);

        public static short? ToNullableInt16(object value, IFormatProvider provider) =>
            ToNullable<short>(value, provider, ToInt16Internal);

        public static short? ToNullableInt16(object value) =>
            ToNullable<short>(value, CultureInfo.CurrentCulture, ToInt16Internal);

        public static short ToInt16(object value, IFormatProvider provider) =>
            To<short>(value, provider, ToInt16Internal);

        public static short ToInt16(object value) =>
            To<short>(value, CultureInfo.CurrentCulture, ToInt16Internal);

        #endregion

        #region Int32

        public static int ToInt32Internal(object value, IFormatProvider provider) =>
            Convert.ToInt32(value, provider);

        public static int? ToNullableInt32(object value, IFormatProvider provider) =>
            ToNullable<int>(value, provider, ToInt32Internal);

        public static int? ToNullableInt32(object value) =>
            ToNullable<int>(value, CultureInfo.CurrentCulture, ToInt32Internal);

        public static int ToInt32(object value, IFormatProvider provider) =>
            To<int>(value, provider, ToInt32Internal);

        public static int ToInt32(object value) =>
            To<int>(value, CultureInfo.CurrentCulture, ToInt32Internal);

        #endregion

        #region Int64

        public static long ToInt64Internal(object value, IFormatProvider provider) =>
            Convert.ToInt64(value, provider);

        public static long? ToNullableInt64(object value, IFormatProvider provider) =>
            ToNullable<long>(value, provider, ToInt64Internal);

        public static long? ToNullableInt64(object value) =>
            ToNullable<long>(value, CultureInfo.CurrentCulture, ToInt64Internal);

        public static long ToInt64(object value, IFormatProvider provider) =>
            To<long>(value, provider, ToInt64Internal);

        public static long ToInt64(object value) =>
            To<long>(value, CultureInfo.CurrentCulture, ToInt64Internal);

        #endregion

        #region Byte

        public static byte ToByteInternal(object value, IFormatProvider provider) =>
            Convert.ToByte(value, provider);

        public static byte? ToNullableByte(object value, IFormatProvider provider) =>
            ToNullable<byte>(value, provider, ToByteInternal);

        public static byte? ToNullableByte(object value) =>
            ToNullable<byte>(value, CultureInfo.CurrentCulture, ToByteInternal);

        public static byte ToByte(object value, IFormatProvider provider) =>
            To<byte>(value, provider, ToByteInternal);

        public static byte ToByte(object value) =>
            To<byte>(value, CultureInfo.CurrentCulture, ToByteInternal);

        #endregion

        #region UInt16

        public static ushort ToUInt16Internal(object value, IFormatProvider provider) =>
            Convert.ToUInt16(value, provider);

        public static ushort? ToNullableUInt16(object value, IFormatProvider provider) =>
            ToNullable<ushort>(value, provider, ToUInt16Internal);

        public static ushort? ToNullableUInt16(object value) =>
            ToNullable<ushort>(value, CultureInfo.CurrentCulture, ToUInt16Internal);

        public static ushort ToUInt16(object value, IFormatProvider provider) =>
            To<ushort>(value, provider, ToUInt16Internal);

        public static ushort ToUInt16(object value) =>
            To<ushort>(value, CultureInfo.CurrentCulture, ToUInt16Internal);

        #endregion

        #region UInt32

        public static uint ToUInt32Internal(object value, IFormatProvider provider) =>
            Convert.ToUInt32(value, provider);

        public static uint? ToNullableUInt32(object value, IFormatProvider provider) =>
            ToNullable<uint>(value, provider, ToUInt32Internal);

        public static uint? ToNullableUInt32(object value) =>
            ToNullable<uint>(value, CultureInfo.CurrentCulture, ToUInt32Internal);

        public static uint ToUInt32(object value, IFormatProvider provider) =>
            To<uint>(value, provider, ToUInt32Internal);

        public static uint ToUInt32(object value) =>
            To<uint>(value, CultureInfo.CurrentCulture, ToUInt32Internal);

        #endregion

        #region UInt64

        public static ulong ToUInt64Internal(object value, IFormatProvider provider) =>
            Convert.ToUInt64(value, provider);

        public static ulong? ToNullableUInt64(object value, IFormatProvider provider) =>
            ToNullable<ulong>(value, provider, ToUInt64Internal);

        public static ulong? ToNullableUInt64(object value) =>
            ToNullable<ulong>(value, CultureInfo.CurrentCulture, ToUInt64Internal);

        public static ulong ToUInt64(object value, IFormatProvider provider) =>
            To<ulong>(value, provider, ToUInt64Internal);

        public static ulong ToUInt64(object value) =>
            To<ulong>(value, CultureInfo.CurrentCulture, ToUInt64Internal);

        #endregion

        #region Decimal

        public static decimal ToDecimalInternal(object value, IFormatProvider provider) =>
            Convert.ToDecimal(value, provider);

        public static decimal? ToNullableDecimal(object value, IFormatProvider provider) =>
            ToNullable<decimal>(value, provider, ToDecimalInternal);

        public static decimal? ToNullableDecimal(object value) =>
            ToNullable<decimal>(value, CultureInfo.CurrentCulture, ToDecimalInternal);

        public static decimal ToDecimal(object value, IFormatProvider provider) =>
            To<decimal>(value, provider, ToDecimalInternal);

        public static decimal ToDecimal(object value) =>
            To<decimal>(value, CultureInfo.CurrentCulture, ToDecimalInternal);

        #endregion

        #region Single

        public static float ToSingleInternal(object value, IFormatProvider provider) =>
            Convert.ToSingle(value, provider);

        public static float? ToNullableSingle(object value, IFormatProvider provider) =>
            ToNullable<float>(value, provider, ToSingleInternal);

        public static float? ToNullableSingle(object value) =>
            ToNullable<float>(value, CultureInfo.CurrentCulture, ToSingleInternal);

        public static float ToSingle(object value, IFormatProvider provider) =>
            To<float>(value, provider, ToSingleInternal);

        public static float ToSingle(object value) =>
            To<float>(value, CultureInfo.CurrentCulture, ToSingleInternal);

        #endregion

        #region Double

        public static double ToDoubleInternal(object value, IFormatProvider provider) =>
            Convert.ToDouble(value, provider);

        public static double? ToNullableDouble(object value, IFormatProvider provider) =>
            ToNullable<double>(value, provider, ToDoubleInternal);

        public static double? ToNullableDouble(object value) =>
            ToNullable<double>(value, CultureInfo.CurrentCulture, ToDoubleInternal);

        public static double ToDouble(object value, IFormatProvider provider) =>
            To<double>(value, provider, ToDoubleInternal);

        public static double ToDouble(object value) =>
            To<double>(value, CultureInfo.CurrentCulture, ToDoubleInternal);

        #endregion

        #region Char

        public static char ToCharInternal(object value, IFormatProvider provider)
        {
            var s = value as string;
            if (s.HasValue())
            {
                return s[0];
            }

            return Convert.ToChar(value, provider);
        }

        public static char? ToNullableChar(object value, IFormatProvider provider) =>
            ToNullable<char>(value, provider, ToCharInternal);

        public static char? ToNullableChar(object value) =>
            ToNullable<char>(value, CultureInfo.CurrentCulture, ToCharInternal);

        public static char ToChar(object value, IFormatProvider provider) =>
            To<char>(value, provider, ToCharInternal);

        public static char ToChar(object value) =>
            To<char>(value, CultureInfo.CurrentCulture, ToCharInternal);

        #endregion

        #region DateTime

        public static DateTime ToDateTimeInternal(object value, IFormatProvider provider) =>
            Convert.ToDateTime(value, provider);

        public static DateTime? ToNullableDateTime(object value, IFormatProvider provider) =>
            ToNullable<DateTime>(value, provider, ToDateTimeInternal);

        public static DateTime? ToNullableDateTime(object value) =>
            ToNullable<DateTime>(value, CultureInfo.CurrentCulture, ToDateTimeInternal);

        public static DateTime ToDateTime(object value, IFormatProvider provider) =>
            To<DateTime>(value, provider, ToDateTimeInternal);

        public static DateTime ToDateTime(object value) =>
            To<DateTime>(value, CultureInfo.CurrentCulture, ToDateTimeInternal);

        #endregion

        #region DateTimeOffset

        public static DateTimeOffset ToDateTimeOffsetInternal(object value, IFormatProvider provider, DateTimeStyles styles)
        {
            if (value is DateTime)
            {
                return (DateTimeOffset)value;
            }

            return DateTimeOffset.Parse(value.ToString(), provider, styles);
        }

        public static DateTimeOffset ToDateTimeOffsetInternal(object value, IFormatProvider provider) =>
            ToDateTimeOffsetInternal(value, provider, DateTimeStyles.None);

        public static DateTimeOffset? ToNullableDateTimeOffset(object value, IFormatProvider provider) =>
            ToNullable<DateTimeOffset>(value, provider, ToDateTimeOffsetInternal);

        public static DateTimeOffset? ToNullableDateTimeOffset(object value) =>
            ToNullable<DateTimeOffset>(value, CultureInfo.CurrentCulture, ToDateTimeOffsetInternal);

        public static DateTimeOffset ToDateTimeOffset(object value, IFormatProvider provider) =>
            To<DateTimeOffset>(value, provider, ToDateTimeOffsetInternal);

        public static DateTimeOffset ToDateTimeOffset(object value) =>
            To<DateTimeOffset>(value, CultureInfo.CurrentCulture, ToDateTimeOffsetInternal);

        #endregion

        #region String

        public static string ToStringInternal(object value, IFormatProvider provider) =>
            Convert.ToString(value, provider);

        public static string ToString(object value, IFormatProvider provider) =>
            To<string>(value, provider, ToStringInternal);

        public static string ToString(object value) =>
            To<string>(value, CultureInfo.CurrentCulture, ToStringInternal);

        #endregion

        #region Guid

        // This method accepts IFormatProvider just to match the signature required by
        // To<T> and ToNullable<T>, but it is not used
        public static Guid ToGuidInternal(object value, IFormatProvider provider)
        {
            var bytes = value as byte[];
            if (null != bytes)
            {
                return new Guid(bytes);
            }

            return new Guid(value.ToString());
        }

        public static Guid? ToNullableGuid(object value) =>
            ToNullable<Guid>(value, null, ToGuidInternal);

        public static Guid ToGuid(object value) =>
            To<Guid>(value, null, ToGuidInternal);

        #endregion

        #region ByteArray

        public static byte[] ToByteArray(object value)
        {
            if (value is byte[])
            {
                return (byte[])value;
            }

            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            throw new FormatException("Cannot cast value to byte[]. ");
        }

        #endregion

        #region To

        public static T To<T>(object value) =>
            To<T>(value, CultureInfo.CurrentCulture);

        public static T To<T>(object value, IFormatProvider provider)
        {
            if (value is T)
            {
                return (T)value;
            }

            if (null == value || DBNull.Value == value)
            {
                return default(T);
            }

            return (T)To(typeof(T), value, provider);
        }

        public static object To(Type type, object value) =>
            To(type, value, CultureInfo.CurrentCulture);

        public static object To(Type type, object value, IFormatProvider provider)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (null != value && value.GetType() == type)
            {
                return value;
            }

            var isNullable = DBConvert.IsNullable(type);
            if (isNullable)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (null == value ||
                DBNull.Value == value)
            {
                if (isNullable || false == type.IsValueType)
                {
                    return null;
                }

                return Activator.CreateInstance(type);
            }

            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return ToBooleanInternal(value, provider);
                case TypeCode.SByte:
                    return ToSByteInternal(value, provider);
                case TypeCode.Int16:
                    return ToInt16Internal(value, provider);
                case TypeCode.Int32:
                    return ToInt32Internal(value, provider);
                case TypeCode.Int64:
                    return ToInt64Internal(value, provider);
                case TypeCode.Byte:
                    return ToByteInternal(value, provider);
                case TypeCode.UInt16:
                    return ToUInt16Internal(value, provider);
                case TypeCode.UInt32:
                    return ToUInt32Internal(value, provider);
                case TypeCode.UInt64:
                    return ToUInt64Internal(value, provider);
                case TypeCode.Decimal:
                    return ToDecimalInternal(value, provider);
                case TypeCode.Single:
                    return ToSingleInternal(value, provider);
                case TypeCode.Double:
                    return ToDoubleInternal(value, provider);
                case TypeCode.Char:
                    return ToCharInternal(value, provider);
                case TypeCode.DateTime:
                    return ToDateTimeInternal(value, provider);
                case TypeCode.String:
                    return ToStringInternal(value, provider);
                case TypeCode.Object:
                    if (type == typeof(Guid))
                    {
                        return ToGuidInternal(value, null);
                    }

                    if (type == typeof(byte[]))
                    {
                        return ToByteArray(value);
                    }

                    if (type == typeof(DateTimeOffset))
                    {
                        return ToDateTimeOffsetInternal(value, null);
                    }

                    break;
            }

            // fallback to System.Convert for IConvertible types
            return Convert.ChangeType(value, typeCode, provider);
        }

        #endregion

        #region ToDBValue

        public static object ToDBValue(object value)
        {
            if (null == value)
            {
                return DBNull.Value;
            }

            return value;
        }

        #endregion

        #region Internal public Helpers

        public static T? ToNullable<T>(object value, IFormatProvider provider, Func<object, IFormatProvider, T> converter)
            where T : struct
        {
            if (value is T)
            {
                return (T)value;
            }

            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            return converter(value, provider);
        }

        public static T To<T>(object value, IFormatProvider provider, Func<object, IFormatProvider, T> converter)
        {
            if (value is T)
            {
                return (T)value;
            }

            if (null == value || DBNull.Value == value)
            {
                return default(T);
            }

            return converter(value, provider);
        }

        public static bool IsNullable(Type type) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>);

        #endregion
    }
}
