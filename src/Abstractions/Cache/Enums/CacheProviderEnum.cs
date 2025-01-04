using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Cache.Enums
{
    public enum CacheProviderEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "RedisCache")]
        Redis = 1,

        [Display(Name = "Memcached")]
        Memcached = 2,

        [Display(Name = "LocalCache")]
        Local = 3,

        [Display(Name = "ElastiCache")]
        Elasti = 4, // AWS

        [Display(Name = "MemoryStore")]
        MemoryStore = 5, // GCP

        [Display(Name = "CouchBase")]
        CouchBase = 6,
    }
}
