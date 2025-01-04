namespace Nwpie.Foundation.Abstractions.Measurement.Models
{

    /// <summary>
    /// Influx DB Fields.
    /// e.g.
    /// "CPU Usage":10
    /// "RequestPerSec":32
    /// </summary>
    public class Field
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

}
