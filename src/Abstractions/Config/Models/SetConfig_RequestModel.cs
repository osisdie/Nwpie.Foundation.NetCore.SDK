using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class SetConfig_RequestModel
    {
        [Required]
        public string ConfigKey { get; set; }
        [Required]
        public string RawData { get; set; }

        public bool? NeedEncrypt { get; set; }
        public string VersionDisplay { get; set; }
    }
}
