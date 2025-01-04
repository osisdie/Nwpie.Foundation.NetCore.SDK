using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class InputExtension
    {
        public static T AssignInRange<T>(this T value, T min, T max)
            where T : struct, IComparable
        {
            return value.CompareTo(min) < 0
                ? min
                : value.CompareTo(max) > 0 ? max : value;
        }

        public static T? AssignInRange<T>(this T? value, T min, T max)
            where T : struct, IComparable
        {
            return (false == value.HasValue)
                ? min
                : (value.Value.CompareTo(min) < 0
                    ? min
                    : value.Value.CompareTo(max) > 0
                        ? max
                        : value
                    );
        }

        public static T AssignIf<T>(this T value, Func<T, bool> condition, T newVal)
        {
            return (null != condition && condition(value))
                ? newVal
                : value;
        }

        public static T? AssignIf<T>(this T? value, Func<T?, bool> condition, T? newVal)
            where T : struct, IComparable
        {
            return (null != condition && condition(value))
                ? newVal
                : value;
        }

        public static T AssignIfNotSet<T>(this T value, T newVal, bool allowEmptyString = false)
        {
            if (typeof(T) == typeof(string))
            {
                if (true == allowEmptyString && null == value)
                {
                    return newVal;
                }

                if (false == allowEmptyString && string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return newVal;
                }

                return value;
            }

            return (EqualityComparer<T>.Default.Equals(value, default(T)))
                ? newVal
                : value;
        }

        public static T? AssignIfNotSet<T>(this T? value, T? newVal)
            where T : struct, IComparable
        {
            return (false == value.HasValue || EqualityComparer<T>.Default.Equals(value.Value, default(T)))
                ? newVal
                : value;
        }
    }
}
