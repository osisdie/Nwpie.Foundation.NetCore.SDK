using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.GroupResource
{
    public class ResGroupResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string OwnerAccountId { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(IsRequired = true)]
        public List<string> ParentObjIds { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(IsRequired = true)]
        public List<string> ChildObjIds { get; set; }
    }
}
