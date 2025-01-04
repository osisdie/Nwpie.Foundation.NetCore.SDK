namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class ElasticSearch_Option : CloudProfile_Option
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string IndexFormat { get; set; }
        public int ConnectionTimeout { get; set; } // Seconds
        public string ConnectionString { get; set; }
    }
}
