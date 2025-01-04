using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListResource
{
    public class ResListResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string OwnerAccountId { get; set; } // parent

        #region optional
        public string ParentObjId { get; set; }
        public string DisplayName { get; set; } // partial match
        public List<string> ObjIds { get; set; }
        public List<string> SrcIds { get; set; }
        public List<string> Tags { get; set; }
        public int? Hierarchy { get; set; } // currently up to 5

        [ApiMember(Description = "Specific account ID binds the resource")]
        public override string AccountId { get; set; } // child
        #endregion
    }
}
