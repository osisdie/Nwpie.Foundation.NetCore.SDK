using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Abstractions.Logging.Extensions
{
    public static class LoggerExtension
    {
        public static IDictionary<string, T> BeginCollection<T>() =>
            new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        public static IDictionary<string, T> TryAdd<T>(this IDictionary<string, T> src, string key, T val)
        {
            src[key] = val;
            return src;
        }

        public static IDictionary<string, T> AddApiKey<T>(this IDictionary<string, T> src)
        {
            src[SysLoggerKey.ApiKey] = (T)Convert.ChangeType(SdkRuntime.ApiKey, typeof(T));
            return src;
        }

        public static IDictionary<string, T> AddDebugData<T>(this IDictionary<string, T> src)
        {
            src[SysLoggerKey.ApiName] = (T)Convert.ChangeType(SdkRuntime.ApiName, typeof(T));
            src[SysLoggerKey.Service] = (T)Convert.ChangeType(SdkRuntime.ServiceName, typeof(T));
            src[SysLoggerKey.Version] = (T)Convert.ChangeType(SdkRuntime.Version, typeof(T));
            src[SysLoggerKey.Environment] = (T)Convert.ChangeType(SdkRuntime.ASPNETCORE_ENVIRONMENT, typeof(T));
            src[SysLoggerKey.IP] = (T)Convert.ChangeType(SdkRuntime.IP, typeof(T));
            src[SysLoggerKey.HostName] = (T)Convert.ChangeType(SdkRuntime.MachineName, typeof(T));
            src[SysLoggerKey.Platform] = (T)Convert.ChangeType(Environment.OSVersion.Platform.ToString(), typeof(T));
            src[SysLoggerKey.TimeStamp] = (T)Convert.ChangeType(DateTime.UtcNow.ToString("s"), typeof(T));
            src[SysLoggerKey.UpTime] = (T)Convert.ChangeType(SdkRuntime._ts.ToString("s"), typeof(T));

            return src;
        }

        public static IDictionary<string, T> AddTraceData<T>(this IDictionary<string, T> src, DateTime? startTick = null)
        {
            if (startTick.HasValue && startTick.Value > DateTime.MinValue)
            {
                src[SysLoggerKey.ElapsedSeconds] = (T)Convert.ChangeType((DateTime.UtcNow - startTick.Value).TotalSeconds.ToString(), typeof(T));
            }

            return src.AddApiKey().AddDebugData();
        }
    }
}
