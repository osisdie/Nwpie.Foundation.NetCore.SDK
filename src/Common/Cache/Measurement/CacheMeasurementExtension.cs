using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nwpie.Foundation.Abstractions.Cache.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Common.Cache.Measurement
{
    public static class CacheMeasurementExtension
    {
        static CacheMeasurementExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(CacheMeasurementExtension));
            m_Serializer = new DefaultSerializer();

            CollectUsageMetricEnabled = ServiceContext.Config
                .CACHE?.CollectUsageMetricEnabled
                ?? false;
            RemoteLoggingEnabled = ServiceContext.Config
                .CACHE?.RemoteLoggingEnabled
                ?? false;
            MeasurementEnabled = ServiceContext.Config
                .CACHE?.MeasurementEnabled
                ?? false;
            if (MeasurementEnabled)
            {
                m_MeasurementClient = ComponentMgr.Instance.TryResolve<IMeasurement>();
            }
        }

        private static bool Write(
            string metricSuffix,
            string realCacheKey,
            CacheProviderEnum provider,
            DateTime? startTick,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (false == IsValid)
            {
                return false;
            }

            var fields = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { $"cache.{metricSuffix}", 1 }
            };

            if (startTick.HasValue)
            {
                fields.Add($"cache.duration.ms", (DateTime.UtcNow - startTick.Value).TotalMilliseconds);
            }

            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (realCacheKey.HasValue())
            {
                tags.Add("cache.key", realCacheKey);
            }

            //var data = new Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>>();
            //data[$"cache.{metricSuffix}"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Count, 1) };
            //data[$"cache.{metricSuffix}.{realCacheKey}"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Count, 1) };

            //var _ = MeasurementClient.WriteAsync(data,
            //    new Dictionary<string, string>() {
            //        { $"{provider}Cache", provider.ToString() },
            //    }
            //);

            m_MeasurementClient?.WritePoint($"{provider.GetDisplayName()}", fields, tags);
            return true;
        }

        public static bool WriteCacheMiss(
            string realCacheKey,
            CacheProviderEnum provider,
            DateTime? startTick,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("miss", realCacheKey, provider, startTick, caller, srcFilePath);

        public static bool WriteCacheHit(
            string realCacheKey,
            CacheProviderEnum provider,
            DateTime? startTick,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (CollectUsageMetricEnabled)
            {
                return Write("hit", realCacheKey, provider, startTick, caller, srcFilePath);
            }

            return true;
        }

        public static bool WriteCacheException(
            string realCacheKey,
            CacheProviderEnum provider,
            DateTime? startTick,
            Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.CacheException.GetDisplayName() },
                { SysLoggerKey.Provider, provider },
                { SysLoggerKey.CacheKey, realCacheKey },
                { SysLoggerKey.Exception, ex?.ToString() },
                { SysLoggerKey.Caller, caller },
                { SysLoggerKey.SrcFilePath, srcFilePath },
            }.AddTraceData()));

            return Write("exception", realCacheKey, provider, startTick, caller, srcFilePath);
        }

        public static ILogger Logger;
        public static bool MeasurementEnabled;
        public static bool RemoteLoggingEnabled;
        public static bool CollectUsageMetricEnabled = false;

        private static readonly ISerializer m_Serializer;
        private static readonly IMeasurement m_MeasurementClient;
        private static bool IsValid => MeasurementEnabled && null != m_MeasurementClient;
    }
}
