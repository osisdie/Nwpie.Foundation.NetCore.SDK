using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ChangePassword
{
    public class AcctChangePassword_RequestModel : AuthParamModel
    {
        [ApiMember(Description = "Current password (old)")]
        //[CustomIgnoreLogging(Description = "Suppress")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [RegularExpression(ConfigConst.PasswordPattern)]
        [ApiMember(IsRequired = true)]
        //[CustomIgnoreLogging(Description = "Suppress")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        [ApiMember(IsRequired = true)]
        //[CustomIgnoreLogging(Description = "Suppress")]
        public string ConfirmPassword { get; set; }
    }
}
