using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.DeleteToken
{
    public class TkDeleteToken_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(IsRequired = true)]
        public string AccessKey { get; set; } // ApiName, PAT, Bearer Token

        #region optional
        public override string AccountId { get; set; }
        #endregion
    }
}
