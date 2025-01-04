using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.ValidateApiKey
{
    public class TkValidateApiKey_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(Description = "Forward requester's ApiKey", IsRequired = true)]
        public string ApiKey { get; set; }
    }
}
