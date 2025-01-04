using System.Collections.Generic;

namespace Nwpie.Foundation.Common.Measurement
{
    public class MetricSerie
    {
        public IList<string> Columns { get; set; }
        public string Name { get; set; }
        public IDictionary<string, string> Tags { get; set; }
        public IList<IList<object>> Values { get; set; }
    }
}
