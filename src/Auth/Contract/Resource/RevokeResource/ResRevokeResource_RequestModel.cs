using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.RevokeResource
{
    public class ResRevokeResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string OwnerAccountId { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Resource GUID", IsRequired = true)]
        public List<string> ObjIds { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Revoke this resource from whom ?", IsRequired = true)]
        public List<string> AccountIds { get; set; }
    }
}
