using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Contract.Login
{
    public class AcctLogin_RequestModel : RequestDtoBase
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [EmailAddress]
        [ApiMember(IsRequired = true)]
        public string Email { get; set; }

        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [ApiMember(IsRequired = true)]
        //[IgnoreLogging(Description = "Suppress")]
        public string Password { get; set; }
    }
}
