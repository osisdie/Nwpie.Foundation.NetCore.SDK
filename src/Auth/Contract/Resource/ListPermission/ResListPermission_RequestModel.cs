using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListPermission
{
    public class ResListPermission_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Application GUID", IsRequired = true)]
        public string AppId { get; set; }
    }
}
