using System.Collections.Generic;

namespace Nwpie.MiniSite.ES.Contract.Models
{
    public class ESProxyResponse<T>
        where T : class
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public ESProxyResponseShards _shards { get; set; }
        public ESProxyResponseHits<T> hits { get; set; }
    }

    public class ESProxyResponseShards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class ESProxyResponseTotal
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class ESProxyResponseHitItem<T>
        where T : class
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public double _score { get; set; }
        public T _source { get; set; }
    }

    public class ESProxyResponseHits<T>
        where T : class
    {
        public ESProxyResponseTotal total { get; set; }
        public double max_score { get; set; }
        public List<ESProxyResponseHitItem<T>> hits { get; set; }
    }
}
