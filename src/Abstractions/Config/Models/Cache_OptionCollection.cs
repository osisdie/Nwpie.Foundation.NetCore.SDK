namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Cache_OptionCollection : OptionBase
    {
        public string Kind { get; set; }
        public int DefaultDurationSecs { get; set; }
        public RedisCache_Option Redis { get; set; }
    }
}
