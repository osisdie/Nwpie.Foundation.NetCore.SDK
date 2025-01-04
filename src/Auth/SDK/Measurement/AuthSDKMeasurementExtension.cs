using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Auth.SDK.Measurement
{
    public static class AuthSDKMeasurementExtension
    {
        static AuthSDKMeasurementExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(AuthSDKMeasurementExtension));
            m_Serializer = new DefaultSerializer();

            CollectUsageMetricEnabled = ServiceContext.Config
                .AUTH?.CollectUsageMetricEnabled
                ?? false;
            RemoteLoggingEnabled = ServiceContext.Config
                .AUTH?.RemoteLoggingEnabled
                ?? false;
            MeasurementEnabled = ServiceContext.Config
                .AUTH?.MeasurementEnabled
                ?? false;
            if (MeasurementEnabled)
            {
                m_MeasurementClient = ComponentMgr.Instance.TryResolve<IMeasurement>();
            }
        }

        private static bool Write(
            string metricSuffix,
            string token,
            string provider,
            string user,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (false == IsValid)
            {
                return false;
            }

            var fields = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { $"token.{metricSuffix}", 1 }
            };

            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (user.HasValue())
            {
                tags.Add("token.user", user);
            }

            if (token.HasValue())
            {
                tags.Add("token.or.md5", token);
            }

            m_MeasurementClient?.WritePoint($"{provider}", fields, tags);
            return true;
        }

        public static bool WriteBadToken(
            string token,
            string provider,
            string user,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("invalid", token, provider, user, caller, srcFilePath);

        public static bool WriteOKToken(
            string token,
            string provider,
            string user,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (CollectUsageMetricEnabled)
            {
                return Write("valid", token, provider, user, caller, srcFilePath);
            }

            return true;
        }

        public static bool WriteExpiredToken(
            string token,
            string provider,
            string user,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("expired", token, provider, user, caller, srcFilePath);

        public static ILogger Logger;
        public static bool MeasurementEnabled;
        public static bool RemoteLoggingEnabled;
        public static bool CollectUsageMetricEnabled = false;

        private static readonly ISerializer m_Serializer;
        private static readonly IMeasurement m_MeasurementClient;
        private static bool IsValid => MeasurementEnabled && null != m_MeasurementClient;
    }
}
