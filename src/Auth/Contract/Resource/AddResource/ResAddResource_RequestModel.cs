using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.AddResource
{
    public class ResAddResource_RequestModel : AuthParamModel
    {
        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Specific owner account ID (application)", IsRequired = true)]
        public string OwnerAccountId { get; set; }

        [Required]
        [ApiMember(Description = "Source's ID belongs to its original application", IsRequired = true)]
        public string SrcId { get; set; }

        [Required]
        [ApiMember(Description = "Permission GUID for read, write, etc", IsRequired = true)]
        public string PermId { get; set; }

        [Required]
        [StringLength(ConfigConst.MaxIdentifierLength)]
        [ApiMember(Description = "Display name", IsRequired = true)]
        public string DisplayName { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Not empty array", IsRequired = true)]
        public List<string> Tags { get; set; }

        #region optional
        public Dictionary<string, string> Metadata { get; set; }

        [StringLength(ConfigConst.MaxTextLength)]
        public string SrcPath { get; set; }

        [StringLength(ConfigConst.MaxTextLength)]
        public string Description { get; set; }

        [StringLength(ConfigConst.MaxIdentifierLength)]
        public string Status { get; set; }

        public bool? IsOverWritten { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsFunc { get; set; }
        public bool? IsPublic { get; set; }
        public string ParentObjId { get; set; } // Auto grouping if needed
        #endregion
    }
}
