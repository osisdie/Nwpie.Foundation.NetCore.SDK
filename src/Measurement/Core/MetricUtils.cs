using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using InfluxData.Net.InfluxDb.Models;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Measurement.Core
{
    internal static class MetricUtils
    {
        static MetricUtils()
        {
            Logger = LogMgr.CreateLogger(typeof(MetricUtils));
        }

        public static List<Point> ToInfluxDBPoints(List<MetricPoint> metricPoints)
        {
            var points = new List<Point>();
            if (true != (metricPoints?.Count() > 0))
            {
                return points;
            }

            try
            {
                foreach (var mpoint in metricPoints)
                {
                    if (false == string.IsNullOrWhiteSpace(mpoint?.Name))
                    {
                        var point = new Point()
                        {
                            Name = mpoint.Name
                        };

                        // Fields property.
                        if (mpoint.Fields?.Count() > 0)
                        {
                            point.Fields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                            foreach (var field in mpoint.Fields)
                            {
                                if (false == string.IsNullOrWhiteSpace(field.Name))
                                {
                                    point.Fields.Add(field.Name, field.Value);
                                }
                            }
                        }

                        // Tag property.
                        if (mpoint.Tags?.Count() > 0)
                        {
                            point.Tags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                            foreach (var tag in mpoint.Tags)
                            {
                                if (false == string.IsNullOrWhiteSpace(tag.Name))
                                {
                                    if (string.IsNullOrWhiteSpace(tag.Value))
                                    {
                                        tag.Value = "NULL";
                                    }

                                    point.Tags.Add(tag.Name, tag.Value);
                                }
                            }
                        }

                        // Timestamp property.
                        if (DateTime.TryParse(mpoint.TimeStamp,
                            null,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                            out var utc_datetime))
                        {
                            if (DateTime.MinValue != utc_datetime &&
                                DateTime.MaxValue != utc_datetime)
                            {
                                point.Timestamp = utc_datetime;
                            }
                            else
                            {
                                point.Timestamp = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            point.Timestamp = DateTime.UtcNow;
                        }

                        points.Add(point);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return points;
        }

        public static readonly ILogger Logger;
    }
}
