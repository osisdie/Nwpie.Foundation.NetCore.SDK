using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.DataAccess.Database.Measurement
{
    public static class CommandExecutorExtension
    {
        static CommandExecutorExtension()
        {
            Logger = LogMgr.CreateLogger(typeof(CommandExecutorExtension));
            m_Serializer = new DefaultSerializer();

            RemoteLoggingEnabled = ServiceContext.Config.DAL?.RemoteLoggingEnabled
                ?? false;
            MeasurementEnabled = ServiceContext.Config.DAL?.MeasurementEnabled
                ?? false;
            if (MeasurementEnabled)
            {
                m_MeasurementClient = ComponentMgr.Instance
                    .TryResolve<IMeasurement>();
            }
        }

        public static async Task<bool> TryExecuteAsync(this CommandExecutor cmder, Func<Task> action, [CallerMemberName] string caller = "")
        {
            var startTick = DateTime.UtcNow;
            Exception lastEx = null;

            try
            {
                await action();

                WriteDalHit(dbName: cmder.CurrentCommand?.CurrentConnection?.Database,
                    cmdName: cmder.CommandName,
                    provider: cmder.CurrentCommand.Provider,
                    elapsedSeconds: (DateTime.UtcNow - startTick).TotalSeconds
                );

                if (RemoteLoggingEnabled)
                {
                    Logger.LogInformation(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { SysLoggerKey.Type, LoggingTypeEnum.DALRequest.GetDisplayName() },
                        { SysLoggerKey.Database, cmder.CurrentCommand?.CurrentConnection?.Database },
                        { SysLoggerKey.CommandText, cmder.CurrentCommand?.CommandText },
                        { SysLoggerKey.CommandType, cmder.CurrentCommand?.CommandType.ToString() },
                        { SysLoggerKey.CommandGuid, cmder.CurrentCommand?._id },
                        { SysLoggerKey.Paramters, cmder.Parameters },
                        { SysLoggerKey.Caller, caller },
                    }.AddTraceData(startTick)));
                }

                return true;
            }
            catch (Exception ex)
            {
                lastEx = ex.GetBaseFirstException();

                var insightInspection = DataAccessUtils.InspectSqlCommand(cmder.CurrentCommand?.CommandText, cmder.Parameters);

                Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { SysLoggerKey.Type, LoggingTypeEnum.DALException.GetDisplayName() },
                    { SysLoggerKey.Database, cmder.CurrentCommand?.CurrentConnection?.Database },
                    { SysLoggerKey.CommandType, cmder.CurrentCommand?.CommandType.ToString() },
                    { SysLoggerKey.CommandGuid, cmder.CurrentCommand?.ConnectionGuid },
                    { SysLoggerKey.Insight, insightInspection },
                    { SysLoggerKey.Caller, caller },
                    { SysLoggerKey.Exception, lastEx?.ToString() },
                }.AddTraceData(startTick)));

                WriteDalException(dbName: cmder.CurrentCommand?.CurrentConnection?.Database,
                    cmdName: cmder.CommandName,
                    provider: cmder.CurrentCommand.Provider,
                    elapsedSeconds: (DateTime.UtcNow - startTick).TotalSeconds,
                    ex: lastEx
                );

                if (ServiceContext.IsDebugOrDevelopment() && insightInspection?.MissingParameters?.Count > 0)
                {
                    throw new ArgumentNullException(string.Join(",", insightInspection?.MissingParameters), $"Missing {insightInspection?.MissingParameters?.Count()} parameter(s) for sql execution. ");
                }

                throw;
            }
        }

        private static bool Write(
            string metricSuffix,
            string dbName,
            string cmdName,
            DataSourceEnum provider,
            double elapsedSeconds,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "")
        {
            if (false == IsValid || null == m_MeasurementClient)
            {
                return false;
            }

            dbName = dbName ?? "default";
            //var data = new Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>>();
            //if (elapsedSeconds > 0)
            //{
            //    data[$"dal.duration"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Seconds, elapsedSeconds) };
            //}
            //data[$"dal.{metricSuffix}"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Count, 1) };
            //data[$"dal.{metricSuffix}.{cmdName}"] = new List<KeyValuePair<MeasurementUnitEnum, double>>() { new KeyValuePair<MeasurementUnitEnum, double>(MeasurementUnitEnum.Count, 1) };

            var fields = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { $"dal.{metricSuffix}", 1 }
            };

            if (elapsedSeconds > 0)
            {
                fields.Add($"dal.duration", elapsedSeconds);
            }

            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (dbName.HasValue())
            {
                tags.Add("dal.database", dbName);
            }

            if (cmdName.HasValue())
            {
                tags.Add("dal.command", cmdName);
            }

            m_MeasurementClient?.WritePoint(provider.ToString(), fields, tags);

            //var _ = MeasurementClient.WriteAsync(data,
            //    new Dictionary<string, string>() {
            //        { provider.ToString(), dbName },
            //    }
            //);

            return true;
        }

        public static bool WriteDalHit(
            string dbName,
            string cmdName,
            DataSourceEnum provider,
            double elapsedSeconds,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("entry", dbName, cmdName, provider, elapsedSeconds, caller, srcFilePath);

        public static bool WriteDalException(
            string dbName,
            string cmdName,
            DataSourceEnum provider,
            double elapsedSeconds,
            Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string srcFilePath = "") =>
            Write("exception", dbName, cmdName, provider, elapsedSeconds, caller, srcFilePath);

        public static ILogger Logger;
        public static bool MeasurementEnabled;
        public static bool RemoteLoggingEnabled;

        private static readonly ISerializer m_Serializer;
        private static readonly IMeasurement m_MeasurementClient;
        private static bool IsValid => MeasurementEnabled && null != m_MeasurementClient;
    }
}
