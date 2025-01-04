using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.CreateAccount
{
    public class AcctCreateAccountByDevice_RequestModel : AcctCreateAccount_RequestModel
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [EmailAddress]
        [ApiMember(IsRequired = true)]
        public override string Email { get; set; }

        [Required]
        [ApiMember(Description = "Android, iOS, etc", IsRequired = true)]
        public string Platform { get; set; }
    }

    public class AcctCreateAccountByEmail_RequestModel : AcctCreateAccount_RequestModel
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [EmailAddress]
        [ApiMember(IsRequired = true)]
        public override string Email { get; set; }
    }

    public class AcctCreateAccountByMobile_RequestModel : AcctCreateAccount_RequestModel
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(IsRequired = true)]
        public override string Mobile { get; set; }

        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(IsRequired = true)]
        public override string CountryCode { get; set; }
    }

    public class AcctCreateAccountByAppName_RequestModel : AcctCreateAccount_RequestModel
    {
        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(ConfigConst.MaxIdentifierLength)]
        [RegularExpression(AuthServiceConfig.AppNamePattern)]
        [ApiMember(IsRequired = true)]
        public override string Name { get; set; } // unique sys_name

        [Required]
        [MinLength(ConfigConst.MinIdentifierLength)]
        [MaxLength(255)]
        [ApiMember(IsRequired = true)]
        public override string DisplayName { get; set; }

        [Required]
        [StringLength(ConfigConst.MaxTextLength)]
        [ApiMember(IsRequired = true)]
        public override string Description { get; set; }
    }

    public class AcctCreateAccount_RequestModel : AuthParamModel
    {
        #region Master (ACCOUNT)
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string Mobile { get; set; }
        #endregion

        #region Detail, optional (ACCOUNT_DETAIL)
        // Grant application's login permission if not empty
        public virtual string AppId { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string NewPassword { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public string Env { get; set; }
        public virtual DateTime? ExpireDate { get; set; }
        public virtual string Description { get; set; }
        public virtual List<string> Tags { get; set; }
        public virtual Dictionary<string, string> Profile { get; set; }
        public virtual Dictionary<string, string> Metadata { get; set; }
        #endregion

        #region Hidden fields
        [IgnoreDataMember]
        public virtual string Password { get; set; }

        [IgnoreDataMember]
        public virtual string Status { get; set; }

        [IgnoreDataMember]
        public virtual bool? IsHidden { get; set; }
        #endregion
    }
}
