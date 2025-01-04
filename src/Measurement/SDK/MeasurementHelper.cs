using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Measurement;
using Nwpie.Foundation.Measurement.SDK.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Measurement.SDK
{
    /// <summary>
    /// Util for writing metrics data to TSDB
    /// Usage:
    /// 1 field without tag.
    /// MeasurementHelper.WritePoint("cpu", "current", 10.0);
    ///
    /// 1 field, 1 tag.
    /// MeasurementHelper.WritePoint("cpu", "current", 10.0, "computer", "LabPC");
    ///
    /// multiple fields, multiple userTags.
    /// MeasurementHelper.WritePoint(
    ///     "sysinfo",
    ///     new Dictionary<string, float>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}},
    ///     new Dictionary<string, string>() { { "computer", "LabPC"}, { "arch", "x64" }});;
    /// </summary>
    public sealed class MeasurementHelper
    {
        static MeasurementHelper()
        {
            Logger = LogMgr.CreateLogger(typeof(MeasurementHelper));
            MeasurementEnabled = ServiceContext.MeasurementTraceEnabled;
        }
        /// <summary>
        /// Write a metirc point.
        /// Example: WritePoint("cpu_usage", "current", 23.0);
        /// </summary>
        /// <param name="metric">name of metric</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        public static void WritePoint(string metric, string key, double value) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            }, null);

        /// <summary>
        /// Write a metirc point with tag.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        public static void WritePoint(string metric, string key, double value, string tagKey, string tagValue) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            }, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { tagKey, tagValue }
            });

        /// <summary>
        /// Write a metirc point with multiple userTags.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        public static void WritePoint(string metric, string key, double value, IDictionary<string, string> tags) =>
            WritePoint(metric, new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                { key, value }
            }, tags);

        /// <summary>
        /// Write a metirc point with multiple fields.
        /// MeasurementHelper.WritePoint(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}});
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        public static void WritePoint(string metric, IDictionary<string, double> fields) =>
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
        public static void WritePoint(string metric, IDictionary<string, double> fields, string tagKey, string tagValue) =>
            WritePoint(metric, fields, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { tagKey, tagValue }
            });

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
        public static void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags) =>
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
        public static void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags, DateTime timestampInUTC)
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
            point.TimeStamp = (timestampInUTC.EscapeMinValue() ?? DateTime.UtcNow).ToString(MMConstant.DateTimeFormatOfMetircPoint);

            SubmitPoint(point);
        }


        #region Helper methods
        public static void BuildFields(List<Field> fieldList, IDictionary<string, double> fromFields)
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
        public static void BuildTags(List<Tag> tagList, IDictionary<string, string> fromTags)
        {
            fromTags = fromTags ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                if (kvp.Key.HasValue() && kvp.Value.HasValue())
                {
                    tagList.Add(new Tag
                    {
                        Name = kvp.Key.ToLower(),
                        Value = kvp.Value
                    });
                }
            }
        }

        /// <summary>
        /// Submits performance point to measurement client engine.
        /// </summary>
        /// <param name="point">Performance point.</param>
        public static void SubmitPoint(MetricPoint record)
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
                        m_Host = new MeasurementHost();
                    }
                }
            }

            m_Host?.PerformWrite(record);
        }

        #endregion

        public static readonly ILogger Logger;
        public static bool MeasurementEnabled { get; set; }

        static readonly object m_Lock = new object();
        static IMeasurementHost m_Host;
    }
}
