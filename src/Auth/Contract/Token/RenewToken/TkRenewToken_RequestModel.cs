using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.RenewToken
{
    public class TkRenewToken_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(Description = "Refresh Token", IsRequired = true)]
        public string Token { get; set; }
    }
}
