using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Measurement
{
    public static class ApiServiceEntryExtension
    {
        static ApiServiceEntryExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(ApiServiceEntryExtension));
            m_Serializer = new DefaultSerializer();

            RemoteLoggingEnabled = ServiceContext.Config.API?.RemoteLoggingEnabled
                ?? false;
            MeasurementEnabled = ServiceContext.Config.API?.MeasurementEnabled
                ?? false;
            if (MeasurementEnabled)
            {
                m_MeasurementClient = ComponentMgr.Instance.TryResolve<IMeasurement>();
            }
        }

        private static bool Write(
            string metricSuffix,
            string host,
            string url,
            DateTime? startTick,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (false == IsValid)
            {
                return false;
            }

            url = url ?? "/";
            //var data = new Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>>();
            //if (startTick.HasValue)
            //{
            //    data[$"api.duration"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Seconds, (DateTime.UtcNow - startTick.Value).TotalSeconds) };
            //}
            //data[$"api.{metricSuffix}"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Count, 1) };

            var fields = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { $"api.{metricSuffix}", 1 }
            };

            if (startTick.HasValue)
            {
                fields.Add($"api.duration.ms", (DateTime.UtcNow - startTick.Value).TotalMilliseconds);
            }

            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (host.HasValue())
            {
                tags.Add("host", host);
            }

            if (url.HasValue())
            {
                tags.Add("url", url);
            }

            // CloudWatch
            //_ = MeasurementClient.WriteAsync(data, new Dictionary<string, string>() {{ "ServiceEntry", url }});

            m_MeasurementClient?.WritePoint($"ServiceEntry", fields, tags);

            return true;
        }

        public static bool WriteApiHit(
            string host,
            string url,
            DateTime? startTick = null,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("entry", host, url, startTick, caller, srcFilePath);

        public static bool WriteApiBadRequest(
            string host,
            string url,
            DateTime? startTick,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("fail", host, url, startTick, caller, srcFilePath);

        public static bool WriteApiException(
            string host,
            string url,
            DateTime? startTick,
            Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.ApiException.GetDisplayName() },
                { SysLoggerKey.HostName, host },
                { SysLoggerKey.Url, url },
                { SysLoggerKey.Caller, caller },
                { SysLoggerKey.SrcFilePath, srcFilePath },
                { SysLoggerKey.Exception, ex?.ToString() },
            }.AddTraceData(startTick)));

            return Write("exception", host, url, startTick, caller, srcFilePath);
        }

        public static bool MeasurementEnabled = true;
        public static bool RemoteLoggingEnabled = true;
        public static ILogger Logger;

        static readonly ISerializer m_Serializer;
        static readonly IMeasurement m_MeasurementClient;
        static bool IsValid => MeasurementEnabled && null != m_MeasurementClient;
    }
}
