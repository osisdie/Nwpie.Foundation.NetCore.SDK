using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StackExchange.Redis;

namespace Nwpie.Foundation.Caching.Redis
{
    public static class StackExchangeRedisUtils
    {
        public static HashEntry[] ToHashEntries(this object obj)
        {
            var properties = obj.GetType().GetProperties();
            var entries = new List<HashEntry>();
            foreach (var property in properties)
            {
                if (null == property.GetValue(obj))
                {
                    continue;
                }

                if (property.PropertyType == typeof(DateTime?))
                {
                    var val = (property.GetValue(obj) as DateTime?)
                        .Value
                        .ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    entries.Add(new HashEntry(property.Name, val));
                    continue;
                }

                entries.Add(new HashEntry(property.Name, property.GetValue(obj).ToString()));
            }

            return entries.ToArray();
        }

        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            var properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                var entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry()))
                {
                    continue;
                }

                if (false == property.PropertyType.IsGenericType)
                {
                    property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
                    continue;
                }

                var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), Nullable.GetUnderlyingType(property.PropertyType)));
                }
            }

            return (T)obj;
        }
    }
}
