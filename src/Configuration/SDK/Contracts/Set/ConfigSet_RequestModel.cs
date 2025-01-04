using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Set
{
    public class ConfigSet_RequestModel : RequestDtoBase
    {
        [Required]
        public string ConfigKey { get; set; }

        [Required]
        public string RawData { get; set; }

        public string VersionDisplay { get; set; }
        public bool NeedEncrypt { get; set; }
        public string ConfigSection { get; set; }
        public bool IsBaseConfig { get; set; }
    }
}
