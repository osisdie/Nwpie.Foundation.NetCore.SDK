using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Permission
{
    public class KvsPermission_RequestModel : RequestDtoBase
    {
        [Required]
        public string ConfigKey { get; set; }

        [Required]
        public KvsPermission_RequestModelItem Permission { get; set; }

        public string FriendAppName { get; set; }
    }
}
