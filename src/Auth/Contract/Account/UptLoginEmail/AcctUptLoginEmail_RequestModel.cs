using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.UptLoginEmail
{
    public class AcctUptLoginEmail_RequestModel : AuthParamModel
    {
        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [ApiMember(IsRequired = true)]
        public string Password { get; set; }

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

        #region Hidden fields
        [IgnoreDataMember]
        public virtual string OldEmail { get; set; }
        #endregion
    }
}
