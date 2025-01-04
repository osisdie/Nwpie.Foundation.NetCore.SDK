namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Measurement_OptionCollection : OptionBase
    {
        public ServiceProfiler_ConfigNode ServiceProfiler { get; set; }
        public SqlProfiler_ConfigNode SqlProfiler { get; set; }
        public CacheProfiler_ConfigNode CacheProfiler { get; set; }
    }

    public class ServiceProfiler_ConfigNode
    {
        public bool Enabled { get; set; }
        public bool Conversation { get; set; }
        //[Range(1, 86400)]
        public int ThresholdSecs { get; set; }
    }

    public class SqlProfiler_ConfigNode
    {
        public bool Enabled { get; set; }
        //[Range(1, 86400)]
        public int ThresholdSecs { get; set; }
    }

    public class CacheProfiler_ConfigNode
    {
        public bool Enabled { get; set; }
        public string ThresholdActions { get; set; }
    }
}
