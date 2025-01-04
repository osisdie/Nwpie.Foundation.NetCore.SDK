using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.ApiKey.Models
{
    public class LocationInfo
    {
        public string ApiKey { get; set; }
        public string Location { get; set; }
        public string EnvCode { get; set; }
        public List<string> IPs { get; set; }
    }
}
