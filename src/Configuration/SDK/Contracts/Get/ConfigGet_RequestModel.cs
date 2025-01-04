using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Validation.Attributes;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Get
{
    public class ConfigGet_RequestModel : RequestDtoBase
    {
        [Required]
        [NotEmptyArray]
        public List<ConfigGet_RequestModelItem> ConfigKeys { get; set; }
    }

    public class ConfigGet_RequestModelItem
    {
        public string FriendAppName { get; set; }
        public string ConfigKey { get; set; }
        public string Version { get; set; }
        public bool ForceFromPlatform { get; set; }
    }
}
