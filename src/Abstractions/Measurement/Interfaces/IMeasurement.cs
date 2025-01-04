using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Models;

namespace Nwpie.Foundation.Abstractions.Measurement.Interfaces
{
    public interface IMeasurement : ICObject
    {
        //Task<bool> WriteAsync(string metricName, List<KeyValuePair<MeasurementUnitEnum, double>> metrics, Dictionary<string, string> dimensions = null, string category = CommonConst.SdkPrefix);
        //Task<bool> WriteAsync(Dictionary<string, List<KeyValuePair<MeasurementUnitEnum, double>>> metrics, Dictionary<string, string> dimensions = null, string category = CommonConst.SdkPrefix);

        /// <summary>
        /// Write a metirc point.
        /// Example: WritePoint("cpu_usage", "current", 23.0);
        /// </summary>
        /// <param name="metric">name of metric</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        void WritePoint(string metric, string key, double value);

        /// <summary>
        /// Write a metirc point with tag.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        void WritePoint(string metric, string key, double value, string tagKey, string tagValue);

        /// <summary>
        /// Write a metirc point with multiple userTags.
        /// Example: WritePoint("cpu_usage", "current", 23.0, "host", "labPC1");
        /// </summary>
        /// <param name="metric">name of metirc</param>
        /// <param name="key">key of field</param>
        /// <param name="value">value of field</param>
        /// <param name="tagKey">key of tag</param>
        /// <param name="tagValue">value of tag</param>
        void WritePoint(string metric, string key, double value, IDictionary<string, string> tags);

        /// <summary>
        /// Write a metirc point with multiple fields.
        /// MeasurementHelper.WritePoint(
        ///     "sysinfo",
        ///     new Dictionary<string, double>() { { "cpu_usage", 32.0}, { "memory_available", 428135487.0}});
        /// </summary>
        /// <param name="metric">metric name</param>
        /// <param name="fields">list of fields in key-value pair format</param>
        void WritePoint(string metric, IDictionary<string, double> fields);

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
        void WritePoint(string metric, IDictionary<string, double> fields, string tagKey, string tagValue);

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
        void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags);

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
        void WritePoint(string metric, IDictionary<string, double> fields, IDictionary<string, string> tags, DateTime timestampInUTC);
    }
}
