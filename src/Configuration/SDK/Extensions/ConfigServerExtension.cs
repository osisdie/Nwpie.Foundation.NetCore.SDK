using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nwpie.Foundation.Common.Config;
using Microsoft.Extensions.Configuration;

namespace Nwpie.Foundation.Configuration.SDK.Extensions
{
    public static class ConfigServerExtension
    {
        public static IConfigurationBuilder FromConfigServer<TConfig>(this IConfigurationBuilder builder, string configKey)
        {
            MapNeedGet.TryAdd(configKey, typeof(TConfig));
            builder.Add(new ConfigServerSource<TConfig>(configKey));

            return builder;
        }

        public static void Retrieved(string configKey, string rawData)
        {
            ConfigEventMgr.Instance.OnConfigGet(configKey, rawData);

            if (MapAlreadyGet.TryAdd(configKey, rawData))
            {
                Console.WriteLine(string.Format(SysMsgConst.SuccessRetrieveConfigValue, configKey));
            }
        }

        public static IEnumerable<string> ConfigKeyList() =>
            MapNeedGet.Keys;

        public static bool IsRetrieved(string configKey) =>
            MapAlreadyGet.ContainsKey(configKey);

        public static string GetValue(string configKey) =>
            MapAlreadyGet[configKey];

        public static readonly ConcurrentDictionary<string, Type> MapNeedGet = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        public static readonly ConcurrentDictionary<string, string> MapAlreadyGet = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public static bool IsFinished { get; set; }
    }
}
