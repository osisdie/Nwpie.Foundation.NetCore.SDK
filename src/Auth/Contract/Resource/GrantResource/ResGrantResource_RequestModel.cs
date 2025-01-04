using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.GrantResource
{
    public class ResGrantResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string OwnerAccountId { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Grant this resource to whom ?", IsRequired = true)]
        public List<string> AccountIds { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Resource GUID", IsRequired = true)]
        public List<string> ObjIds { get; set; }

        [Required]
        [ApiMember(Description = "Permission GUID for read, write, etc", IsRequired = true)]
        public string PermId { get; set; }

        #region optional
        public bool? IsBlocked { get; set; }
        #endregion
    }
}
