using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Measurement;
using Nwpie.Foundation.Common.MessageQueue;
using Nwpie.Foundation.Measurement.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Measurement.Core
{
    /// <summary>
    /// Singleton function class converts performance records and push data to InfluxDB.
    /// </summary>
    public class InfluxDBEngine : CObject, IInfluxDBEngine
    {
        /// Initials InfluxDB connector and backgroud queue workers.
        /// </summary>
        public InfluxDBEngine(IConfigOptions<InfluxDB_Option> option)
        {
            m_Option = option
                ?? throw new ArgumentNullException(nameof(IConfigOptions<InfluxDB_Option>));

            if (ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), m_Option.Value?.Host, m_Option.Value?.Username, m_Option.Value?.Password))
            {
                throw new ArgumentNullException($@"Options
                    {nameof(InfluxDB_Option.Host)},
                    {nameof(InfluxDB_Option.Username)},
                    {nameof(InfluxDB_Option.Password)} are required. ");
            }

            IsSilent = ServiceContext.Config.APM?.IsSilent
                ?? false;
            m_DefaultDB = m_Option.Value.Database
                ?? ServiceContext.Config.APM?.InfluxDefaultDB;
            if (string.IsNullOrWhiteSpace(m_DefaultDB))
            {
                m_DefaultDB = DefaultDB;
            }

            m_InfluxDBManager = new InfluxDBManager(
                m_Option.Value.Host,
                m_Option.Value.Username,
                m_Option.Value.Password
            );

            m_WaitSentQueue = new WorkerQueue<KeyValuePair<string, List<MetricPoint>>>(m_MaxBatchSize, m_FlushInterval);
            m_WaitSentQueue.Flush += SubmitToDB;
        }

        /// <summary>
        /// Public methods exposes for measurement.
        /// </summary>
        /// <param name="points">List of performance records.</param>
        public void Write(List<MetricPoint> points) =>
            Write(m_DefaultDB, points);

        public void Write(string dbName, List<MetricPoint> points)
        {
            ValidateUtils.CheckEmptyString("dbName", dbName);

            try
            {
                if (points?.Count() > 0)
                {
                    m_WaitSentQueue.Enqueue(new KeyValuePair<string, List<MetricPoint>>(dbName, points));
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }
        }

        public async Task<List<MetricSerie>> QueryAsync(string query) =>
            await QueryAsync(m_DefaultDB, query);

        public async Task<List<MetricSerie>> QueryAsync(string dbName, string query)
        {
            ValidateUtils.CheckEmptyString("dbName", dbName);
            ValidateUtils.CheckEmptyString("query", query);

            var result = new List<MetricSerie>();

            try
            {
                var series = await m_InfluxDBManager.QueryAsync(dbName, query);
                if (series?.Count() > 0)
                {
                    foreach (var serie in series)
                    {
                        var metric = new MetricSerie
                        {
                            Name = serie.Name,
                            Columns = serie.Columns,
                            Values = serie.Values,
                            Tags = serie.Tags
                        };

                        result.Add(metric);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return result;
        }

        private async void SubmitToDB(object sender, List<KeyValuePair<string, List<MetricPoint>>> queuedItems)
        {
            if (true != (queuedItems?.Count() > 0))
            {
                return;
            }

            foreach (var item in queuedItems)
            {
                var targetDB = item.Key;
                var points = MetricUtils.ToInfluxDBPoints(item.Value);

                if (points?.Count() > 0)
                {
                    await m_InfluxDBManager.WriteAsync(targetDB, points);
#if DEBUG
                    if (false == IsSilent)
                    {
                        Logger.LogTrace($"InfluxDBEngine.SubmitToDB: async sending {points?.Count()} points to db {targetDB}. ");
                    }
#endif
                }
            }
        }

        public void Dispose()
        {
            m_WaitSentQueue?.Dispose();
        }

        public const string DefaultDB = "foundation_temp";
        public bool IsSilent { get; set; }

        protected readonly IConfigOptions<InfluxDB_Option> m_Option;
        protected readonly WorkerQueue<KeyValuePair<string, List<MetricPoint>>> m_WaitSentQueue;
        protected readonly InfluxDBManager m_InfluxDBManager;

        protected int m_MaxBatchSize = 100;
        protected int m_FlushInterval = 1000; // in ms
        protected string m_DefaultDB;
    }
}
