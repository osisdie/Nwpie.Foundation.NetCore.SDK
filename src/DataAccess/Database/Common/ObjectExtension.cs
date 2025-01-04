using System;

namespace Nwpie.Foundation.DataAccess.Database
{
    public static class TypeExtension
    {
        public static object GetDefault(this Type t)
        {
            return t.IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }

        public static T GetDefault<T>()
        {
            var t = typeof(T);
            return (T)GetDefault(t);
        }

        public static bool IsDefault<T>(T other)
        {
            var defaultValue = GetDefault<T>();
            if (null == other)
            {
                return null == defaultValue;
            }

            return other.Equals(defaultValue);
        }
    }
}
