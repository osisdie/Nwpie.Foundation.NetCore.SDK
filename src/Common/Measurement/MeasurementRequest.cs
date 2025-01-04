using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Measurement.Models;

namespace Nwpie.Foundation.Common.Measurement
{
    public class MeasurementRequest
    {
        public string DBName { get; set; }
        public List<MetricPoint> MetricPoints { get; set; } = new List<MetricPoint>();
    }
}
