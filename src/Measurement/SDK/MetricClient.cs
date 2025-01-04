using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Measurement;
using Nwpie.Foundation.Measurement.SDK.Interfaces;

namespace Nwpie.Foundation.Measurement.SDK
{
    /// <summary>
    /// Util for writing metrics data to TSDB
    /// Usage:
    /// 1 field without tag.
    /// Client.WritePoint("cpu", "current", 10.0);
    ///
    /// 1 field, 1 tag.
    /// Client.WritePoint("cpu", "current", 10.0, "computer", "LabPC");
    ///
    /// multiple fields, multiple userTags.
    /// Client.WritePoint(
    ///     "sysinfo",
    ///     new Dictionary<string, float>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}},
    ///     new Dictionary<string, string>() { { "computer", "LabPC"}, { "arch", "x64" }});;
    /// </summary>
    public class MetricClient : CObject, IMetricClient
    {
        public MetricClient()
        {
            MeasurementEnabled = ServiceContext.MeasurementTraceEnabled;
        }

        /// <summary>
        /// Write a metirc point.
        /// Example: WritePoint("cpu_usage", "current", 23.0);
        /// </summary>
        /// <param name="metric">name of metric</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        public void WritePoint(string metric, string key, double value)
        {
            WritePoint(metric,
                new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase) {
                    { key, value }
                }, null
            );
        }

        /// <summary>
        /// Write a metirc point with tag.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        public void WritePoint(string metric, string key, double value, string tagKey, string tagValue)
        {
            WritePoint(metric,
                new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase) {
                    { key, value }
                },
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    { tagKey, tagValue }
                }
            );
        }

        /// <summary>
        /// Write a metirc point with multiple userTags.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        public void WritePoint(string metric, string key, double value, IDictionary<string, string> tags)
        {
            WritePoint(metric,
                new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase) {
                    { key, value }
                },
                tags
            );
        }

        /// <summary>
        /// Write a metirc point with multiple fields.
        /// MeasurementHelper.WritePoint(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}});
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        public void WritePoint(string metric, IDictionary<string, double> fields) =>
            WritePoint(metric, fields, null);


        /// <summary>
        /// Write a metirc point with multiple fields and a tag.
        /// MeasurementHelper.WritePoint(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}},
        ///     "host", "labPC1");
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        public void WritePoint(string metric, IDictionary<string, double> fields, string tagKey, string tagValue) =>
            WritePoint(metric, fields,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { tagKey, tagValue }
                }
            );

        /// <summary>
        /// Write a metirc point with multiple fields and userTags.
        /// MeasurementHelper.WritePoint(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}},
        ///     new Dictionary<string, string>() { { "host", "LabPC1"}, { "arch", "x64" }});
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        /// <param name="userTags">list of userTags in key-value pair format</param>
        public void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags) =>
            WritePoint(metric, fields, tags, DateTime.UtcNow);

        /// <summary>
        /// Write a metirc point with multiple fields and userTags.
        /// MeasurementHelper.WritePoint(.Info(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}},
        ///     new Dictionary<string, string>() { { "host", "LabPC1"}, { "arch", "x64" }},
        ///     DateTime.UTCNow);;
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        /// <param name="userTags">list of userTags in key-value pair format</param>
        /// <param name="timestampInUTC">timestamp in UTC format</param>
        public void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags, DateTime timestampInUTC)
        {
            if (false == MeasurementEnabled || true != (fields?.Count() > 0))
            {
                return;
            }

            ValidateUtils.CheckEmptyString("metric", metric);
            var point = new MetricPoint()
            {
                Name = metric
            };

            BuildFields(point.Fields, fields);
            BuildTags(point.Tags, tags);
            point.TimeStamp = (timestampInUTC.EscapeMinValue()
                ?? DateTime.UtcNow).ToString(MMConstant.DateTimeFormatOfMetircPoint);

            SubmitPoint(point);
        }

        #region private methods

        private void BuildFields(List<Field> fieldList, IDictionary<string, double> fromFields)
        {
            if (true != (fromFields?.Count() > 0))
            {
                fromFields = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    { $"{ServiceContext.ApiName}.invalid.apm.count", 1 }
                };
            }

            foreach (var kvp in fromFields)
            {
                if (false == string.IsNullOrEmpty(kvp.Key))
                {
                    fieldList.Add(new Field
                    {
                        Name = kvp.Key.ToLower(),
                        Value = kvp.Value
                    });
                }
            }
        }

        /// <summary>
        /// Contructs influx userTags.
        /// </summary>
        /// <param name="tagList">Tag list maintained by foundation data contract.</param>
        /// <param name="tagKey">Tag key.</param>
        /// <param name="tagValue">Tag name.</param>
        private void BuildTags(List<Tag> tagList, IDictionary<string, string> fromTags)
        {
            fromTags = fromTags ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (false == fromTags.ContainsKey(CommonConst.ServiceName))
            {
                fromTags.Add(CommonConst.ServiceName.ToLower(), ServiceContext.ServiceName);
            }

            if (false == fromTags.ContainsKey(CommonConst.ApiName))
            {
                fromTags.Add(CommonConst.ApiName.ToLower(), ServiceContext.ApiName);
            }

            if (false == fromTags.ContainsKey(CommonConst.ServiceEnv))
            {
                fromTags.Add(CommonConst.ServiceEnv.ToLower(), ServiceContext.SdkEnv);
            }

            if (false == fromTags.ContainsKey(CommonConst.ServiceLocalIP))
            {
                fromTags.Add(CommonConst.ServiceLocalIP.ToLower(), NetworkUtils.IP);
            }

            foreach (var kvp in fromTags)
            {
                if (false == string.IsNullOrEmpty(kvp.Key) &&
                    false == string.IsNullOrEmpty(kvp.Value))
                {
                    tagList.Add(new Tag
                    {
                        Name = kvp.Key.ToLower(),
                        Value = kvp.Value
                    }
                    );
                }
            }
        }

        /// <summary>
        /// Submits performance point to measurement client engine.
        /// </summary>
        /// <param name="point">Performance point.</param>
        private void SubmitPoint(MetricPoint record)
        {
            if (false == MeasurementEnabled || null == record)
            {
                return;
            }

            if (null == m_Host)
            {
                lock (m_Lock)
                {
                    if (null == m_Host)
                    {
                        m_Host = new DefaultMeasurementHost();
                    }
                }
            }

            m_Host?.PerformWrite(record);
        }
        #endregion

        public bool MeasurementEnabled { get; set; }

        private static readonly object m_Lock = new object();
        private static IMeasurementHost m_Host;
    }
}
