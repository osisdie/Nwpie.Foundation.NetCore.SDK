using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.HasPermission
{
    public class ResHasPermission_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Application GUID", IsRequired = true)]
        public string AppId { get; set; }

        [Required]
        [ApiMember(Description = "Account GUID", IsRequired = true)]
        public override string AccountId { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Match any permission in the list (format: app-mdl-perm)", IsRequired = true)]
        public List<string> Permissions { get; set; }
    }
}
