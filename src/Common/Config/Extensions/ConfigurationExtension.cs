using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Microsoft.Extensions.Configuration;

namespace Nwpie.Foundation.Common.Config.Extensions
{
    public static class ConfigurationExtension
    {
        public static IEnumerable<KeyValuePair<string, string>> GetKeyValueListInSection(this IConfiguration configuration, string section, Regex pattern)
        {
            return configuration.GetSection(section)
                ?.GetChildren()
                ?.Where(x => pattern.IsMatch(x.Key))
                ?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value));
        }

        public static T GetValue<T>(this IConfiguration configuration, string key, ISerializer serializer)
            where T : class
        {
            var value = configuration.GetValue<string>(key);
            if (value.HasValue())
            {
                return serializer.Deserialize<T>(value);
            }

            return default(T);
        }

        public static string EnvFirstConfig(this string configKey, string defaultVal = null, bool forceFromRemote = false, string remoteBaseUrl = null)
        {
            var configVal = Environment.GetEnvironmentVariable(configKey)?.Trim();
            if (configVal.HasValue())
            {
                return configVal;
            }

            return ConfigServerRawValue(configKey, defaultVal, forceFromRemote, remoteBaseUrl);
        }

        public static string ConfigServerRawValue(this string configKey, string defaultVal = null, bool forceFromRemote = false, string remoteBaseUrl = null)
        {
            if (forceFromRemote && remoteBaseUrl.HasValue())
            {
                // TODO
                // 1.retrieve
                // 2.upsert localcache
                // 3.replace ServiceContext.Configuration
            }

            // TODO:
            // 1.get from localcache
            // 2.get from ServiceContext.Configuration if not in cache

            return ServiceContext.Configuration.GetValue<string>(configKey, defaultVal);
        }

        public static T ConfigServerValue<T>(this string configKey, ISerializer serializer = null, bool forceFromRemote = false, string remoteBaseUrl = null)
            where T : class
        {
            var rawValue = configKey.ConfigServerRawValue(
                forceFromRemote: forceFromRemote,
                remoteBaseUrl: remoteBaseUrl
            );

            if (null == rawValue)
            {
                return default(T);
            }

            if (typeof(T).IsValueTyped() && false == typeof(T).IsNullable())
            {
                return (T)Convert.ChangeType(rawValue, typeof(T));
            }

            return (serializer ?? ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false))
                .Deserialize<T>(rawValue, ignoreException: true);
        }

        public static string ResolveGetActionUrl(this string baseUrl, string controllerName = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return $"{baseUrl}{LocationConst.HttpPathToConfigContractRequest_Get}";
            }

            return $"{baseUrl}/{controllerName}{LocationConst.HttpPathToConfigContractRequest_Get}";
        }

        public static string ResolveSetActionUrl(this string baseUrl, string controllerName = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return $"{baseUrl}{LocationConst.HttpPathToConfigContractRequest_Set}";
            }

            return $"{baseUrl}/{controllerName}{LocationConst.HttpPathToConfigContractRequest_Set}";
        }

        public static string ResolveRefreshActionUrl(this string baseUrl, string controllerName = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return $"{baseUrl}{LocationConst.HttpPathToConfigContractRequest_Refresh}";
            }

            return $"{baseUrl}/{controllerName}{LocationConst.HttpPathToConfigContractRequest_Refresh}";
        }
    }
}
