using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Account.CreateAccount;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ToStandard
{
    public class AcctToStandard_RequestModel : AcctCreateAccount_RequestModel
    {
        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [ApiMember(IsRequired = true)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [RegularExpression(ConfigConst.PasswordPattern)]
        [ApiMember(IsRequired = true)]
        public override string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        [ApiMember(IsRequired = true)]
        public override string ConfirmPassword { get; set; }

        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [EmailAddress]
        [ApiMember(IsRequired = true)]
        public string NewEmail { get; set; }

        [Required]
        [Compare(nameof(NewEmail))]
        [ApiMember(IsRequired = true)]
        public string ConfirmEmail { get; set; }
    }
}
