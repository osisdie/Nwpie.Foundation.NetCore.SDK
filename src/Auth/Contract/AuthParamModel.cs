using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Auth.Contract
{
    public class AuthParamModel : RequestDtoBase
    {
        [IgnoreDataMember]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        public virtual string AccountId { get; set; }

        [IgnoreDataMember]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        public virtual string CredentialKey { get; set; } // Email, Phone, AppName

        [IgnoreDataMember]
        public virtual byte? CredentialKind { get; set; }

        [IgnoreDataMember]
        public virtual bool? IsForce { get; set; }
    }
}
