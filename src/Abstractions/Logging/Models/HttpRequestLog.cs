using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Logging.Models
{
    public class HttpRequestLog
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public object Body { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
    }
}
