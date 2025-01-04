using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.ValidateToken
{
    public class TkValidateToken_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(Description = "Forward user's token", IsRequired = true)]
        public string Token { get; set; }

        #region Forward if needed
        public string UA { get; set; }
        public string IP { get; set; }
        public string Mac { get; set; }
        #endregion

        [ApiMember(Description = "Check permissions in specific Module of Application")]
        public string ModuleSysName { get; set; }

        [ApiMember(Description = "Check the specific permission ")]
        public string PermissionName { get; set; }
    }
}
