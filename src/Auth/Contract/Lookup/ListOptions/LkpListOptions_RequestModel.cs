using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Lookup.ListOptions
{
    public class LkpListOptions_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Group name.", IsRequired = true)]
        public string Category { get; set; }
    }
}
