using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class DictionaryExtension
    {
        public static Dictionary<string, string> ToIgnoreCaseDictionary(this Dictionary<string, string> src)
        {
            return new Dictionary<string, string>(src ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }

        public static bool ContainsKeyIgnoreCase<TValue>(this IDictionary<string, TValue> dict, string key)
        {
            return dict.ContainsKeyIgnoreCase(key, out _);
        }

        public static bool ContainsKeyIgnoreCase<TValue>(this IDictionary<string, TValue> dict, string key, out TValue val)
        {
            val = default(TValue);
            if (null == dict)
            {
                return false;
            }

            var exists = dict.Any(o => string.Equals(o.Key, key, StringComparison.OrdinalIgnoreCase));
            if (exists)
            {
                val = dict.FirstOrDefault(o => string.Equals(o.Key, key, StringComparison.OrdinalIgnoreCase)).Value;
                return true;
            }

            return false;
        }

        public static Dictionary<string, string> MergeDictionary(this Dictionary<string, string> src, Dictionary<string, string> ext)
        {
            if (true != ext?.Count() > 0)
            {
                return src ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var dict = new Dictionary<string, string>(src ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
            foreach (var item in ext)
            {
                dict[item.Key.ToLower()] = item.Value;
            }

            return dict;
        }

        public static KeyValuePair<string, object> PairedWith(this string key, object value) =>
            new KeyValuePair<string, object>(key, value);

        public static string DumpFormat(this IDictionary<string, string> dict, string pattern = "{0} : {1}") =>
            string.Join(Environment.NewLine, dict?.Select(kvp => string.Format(pattern, kvp.Key, kvp.Value)));

        public static string DumpFormat(this IDictionary<string, object> dict, string pattern = "{0} : {1}") =>
            string.Join(Environment.NewLine, dict?.Select(kvp => string.Format(pattern, kvp.Key, kvp.Value?.ToString())));

        public static void CopyFrom(this NameValueCollection to, NameValueCollection from)
        {
            if (from?.Count > 0)
            {
                foreach (var key in from?.AllKeys)
                {
                    to?.Add(key, from[key]);
                }
            }
        }

        public static IDictionary<string, string> ToDictionary(this NameValueCollection from)
        {
            var collection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (null != from)
            {
                foreach (var key in from.AllKeys)
                {
                    collection.Add(key, from[key]);
                }
            }

            return collection;
        }
    }
}
