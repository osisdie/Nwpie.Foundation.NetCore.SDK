using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Logging.Models
{
    public class HttpResponseLog
    {
        public int StatusCode { get; set; }
        public object Body { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
    }
}
