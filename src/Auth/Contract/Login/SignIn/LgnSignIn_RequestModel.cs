using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Login.SignIn
{
    public class LgnSignIn_RequestModel : AuthParamModel
    {
        [ApiMember(Description = "Application GUID")]
        public string AppId { get; set; }

        [ApiMember(Description = "Generic login username (email, mobile, appname, apiname, pat, etc)")]
        public string Name { get; set; }

        //[Required]
        //[MinLength(ConfigConst.MinIdentifierLength)]
        //[MaxLength(ConfigConst.MaxIdentifierLength)]
        //[EmailAddress]
        //[ApiMember(IsRequired = true)]
        public string Email { get; set; }

        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [ApiMember(IsRequired = true)]
        //[IgnoreLogging(Description = "Suppress")]
        public string Password { get; set; }

        public override string CredentialKey { get; set; }

        #region Forward if needed
        [IgnoreDataMember]
        public virtual string UA { get; set; }

        [IgnoreDataMember]
        public virtual string IP { get; set; }

        [IgnoreDataMember]
        public virtual string Mac { get; set; }
        #endregion
    }
}
