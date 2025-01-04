using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class FeatureToggle_Option : OptionBase
    {
        [JsonExtensionData]
        public Dictionary<string, object> ConfigMap { get; set; }
    }
}
