using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.UptResource
{
    public class ResUptResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID", IsRequired = true)]
        public string OwnerAccountId { get; set; }

        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Resource GUID", IsRequired = true)]
        public string ObjId { get; set; }

        #region optional
        [ApiMember(Description = "Permission GUID for read, write, etc")]
        public string PermId { get; set; }
        public string DisplayName { get; set; }
        public string SrcPath { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool? IsBlocked { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        [ApiMember(Description = "Auto grouping if needed")]
        public string ParentObjId { get; set; }
        #endregion
    }
}
