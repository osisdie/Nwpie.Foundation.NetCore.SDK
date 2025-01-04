using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class Enum_Extension
    {
        public static string GetDisplayName<T>(this T e)
            where T : struct, IConvertible
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            return Enum<T>.GetDispValue(e);
        }
    }

    public class Enum<T> where T : struct, IConvertible
    {
        public static IDictionary<object, string> GetPairsWithValue()
        {
            var enumType = Enum.GetUnderlyingType(typeof(T));
            var dict = GetPairs();
            IDictionary<object, string> newResult = new Dictionary<object, string>();
            foreach (var pair in dict)
            {
                var enumValue = Enum.Parse(typeof(T), pair.Key.ToString());
                var currentValue = Convert.ChangeType(enumValue, enumType);
                newResult.Add(currentValue, pair.Value);
            }

            return newResult;
        }

        public static IDictionary<T, string> GetPairs()
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            IDictionary<T, string> result = new Dictionary<T, string>();
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
            {
                var e = (T)Enum.Parse(typeof(T), member.Name);
                var display = (member
                    .GetCustomAttribute(typeof(DisplayAttribute), false)
                    as DisplayAttribute
                )?.Name ?? member.Name;

                result.Add(e, display);
            }

            return result;
        }

        public static IDictionary<string, string> GetDispDescPairs()
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                var e = (T)Enum.Parse(typeof(T), m.Name);
                var description = (m
                    .GetCustomAttribute(typeof(DescriptionAttribute), false)
                    as DescriptionAttribute
                )?.Description ?? string.Empty;

                var display = (m
                    .GetCustomAttribute(typeof(DisplayAttribute), false)
                    as DisplayAttribute
                )?.Name;

                if (false == string.IsNullOrWhiteSpace(display))
                {
                    result.Add(display, description);
                }
            }

            return result;
        }

        public static IDictionary<T, string> GetDescPairs()
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            IDictionary<T, string> result = new Dictionary<T, string>();
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                var e = (T)Enum.Parse(typeof(T), m.Name);
                var description = (m.GetCustomAttribute(typeof(DescriptionAttribute), false)
                    as DescriptionAttribute
                )?.Description ?? string.Empty;

                result.Add(e, description);
            }

            return result;
        }

        public static string GetChinese(T e)
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            var dict = GetPairs();
            return dict[e];
        }

        public static string GetChinese(byte e)
        {
            return GetPairsWithValue()
                ?.Where(o => Convert.ToByte(o.Key) == e)
                ?.Select(o => o.Value)
                ?.FirstOrDefault();
        }

        public static string GetChinese(int e)
        {
            return GetPairsWithValue()
                ?.Where(o => Convert.ToInt32(o.Key) == e)
                ?.Select(o => o.Value)
                ?.FirstOrDefault();
        }

        public static string GetDesc(T e)
        {
            if (false == typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type. ");
            }

            return GetDescPairs()?[e];
        }

        public static string GetDesc(byte e)
        {
            return GetDescPairs()
                ?.Where(o => Convert.ToByte(o.Key) == e)
                ?.Select(o => o.Value)
                ?.FirstOrDefault();
        }

        public static string GetDesc(int e)
        {
            return GetDescPairs()
                ?.Where(o => Convert.ToInt32(o.Key) == e)
                ?.Select(o => o.Value)
                ?.FirstOrDefault();
        }

        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();
            foreach (var field in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), field.Name, false));
            }

            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T TryParse(string value, T defaultVal)
        {
            if (Enum.TryParse<T>(value?.Trim() ?? string.Empty, true, out var t))
            {
                return t;
            }

            return defaultVal;
        }

        public static T ParseFromDisplayAttr(string value)
        {
            var dict = GetPairs();
            foreach (var item in dict)
            {
                if (string.Equals(value, item.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Key;
                }
            }

            throw new Exception($"Enum (={value} not found in {typeof(T).Name}. ");
        }

        public static T TryParseFromDisplayAttr(string value, T defaultVal)
        {
            var dict = GetPairs();
            foreach (var item in dict)
            {
                if (string.Equals(value, item.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Key;
                }
            }

            return defaultVal;
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(fi => fi.Name)
                .ToList();
        }

        public static IList<string> GetDispValues(Enum value)
        {
            return GetNames(value)
                ?.Select(obj => GetDispValue(Parse(obj)))
                ?.ToList();
        }

        static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (var staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    var resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDispValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttributes = fieldInfo
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                as DisplayAttribute[];

            if (null != descriptionAttributes?.First()?.ResourceType)
            {
                return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);
            }

            if (null == descriptionAttributes)
            {
                return string.Empty;
            }

            return (descriptionAttributes.Length > 0)
                ? descriptionAttributes[0].Name
                : value.ToString();
        }
    }
}
