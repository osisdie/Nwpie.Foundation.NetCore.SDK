namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class RedisCache_Option : CloudProfile_Option
    {
        public string ClientName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int DbIndex { get; set; }
        public int PoolSize { get; set; }
        public int ConnectTimeout { get; set; } // ms
        public int SyncTimeout { get; set; } // ms
        public string ConnectionString { get; set; }
    }
}
