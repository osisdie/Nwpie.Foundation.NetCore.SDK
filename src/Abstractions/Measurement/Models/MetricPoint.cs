using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Measurement.Models
{
    /// <summary>
    /// Influx DB raw data.
    /// eg.
    /// cpu current=10
    /// cpu current=10, mean=12
    /// cpu, serverName="mypc" current=20
    /// Cache hit=300 miss=10
    /// Cache, server="pc1" hit=200
    /// Log current=500
    /// Log, from="appkey1" current=32432
    /// </summary>
    public class MetricPoint
    {
        #region properties
        public string Name { get; set; }

        /// <summary>
        /// Collection of fields.
        /// </summary>
        public List<Field> Fields { get; set; } = new List<Field>();

        /// <summary>
        /// Tags of point, generally used for query.
        /// </summary>
        public List<Tag> Tags { get; set; } = new List<Tag>();

        /// <summary>
        /// Timestamp of this point.
        /// </summary>
        public string TimeStamp { get; set; }
        #endregion
    }
}
