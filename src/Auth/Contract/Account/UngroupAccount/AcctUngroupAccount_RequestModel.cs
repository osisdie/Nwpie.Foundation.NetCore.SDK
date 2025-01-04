using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.UngroupAccount
{
    public class AcctUngroupAccount_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string ParentAccountId { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(IsRequired = true)]
        public List<string> ChildAccountIds { get; set; }
    }
}
