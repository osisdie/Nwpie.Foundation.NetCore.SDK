using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.RecoveryPassword
{
    public class AcctRecoveryPassword_RequestModel : AuthParamModel
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [EmailAddress]
        [ApiMember(IsRequired = true)]
        public string Email { get; set; }

        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [ApiMember(Description = "Email's temp password", IsRequired = true)]
        //[CustomIgnoreLogging(Description = "Suppress")]
        public string TempPassword { get; set; }

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
