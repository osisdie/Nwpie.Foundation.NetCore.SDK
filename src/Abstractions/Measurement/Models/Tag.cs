namespace Nwpie.Foundation.Abstractions.Measurement.Models
{

    /// <summary>
    /// Influx DB Tags.
    /// e.g.
    /// "ServerIP":"192.168.111.100"
    /// "UserName":"UserA"
    /// </summary>
    public class Tag
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
