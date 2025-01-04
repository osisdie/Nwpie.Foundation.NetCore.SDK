using System;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsValueTyped(this Type type)
        {
            return type.IsPrimitive ||
                type.IsValueType ||
                type == typeof(string);
        }

        public static bool IsNullable(this Type type) =>
            null != Nullable.GetUnderlyingType(type);

        public static T ToScalar<T>(this object src, T defaultVal = default(T))
            where T : IComparable
        {
            if (null == src)
            {
                return defaultVal;
            }

            var srcString = src.ToString();
            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(srcString.ToBool(), typeof(T));
            }

            return (T)Convert.ChangeType(src, typeof(T));
        }
    }
}
