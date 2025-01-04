using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Validation.Attributes;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Get
{
    public class KvsGet_RequestModel : RequestDtoBase
    {
        [Required]
        [NotEmptyArray]
        public List<KvsGet_RequestModelItem> ConfigKeys { get; set; }
    }

    public class KvsGet_RequestModelItem
    {
        public string FriendAppName { get; set; }
        public string ConfigKey { get; set; }
        public string Version { get; set; }
        public bool ForceFromPlatform { get; set; }
    }
}
