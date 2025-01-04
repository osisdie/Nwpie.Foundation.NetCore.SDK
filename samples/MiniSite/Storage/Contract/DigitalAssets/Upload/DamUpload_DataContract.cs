using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.Contract.Assets.Upload
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Storage:Private Upload")]
    [Route("/Upload", "POST")]
    public class DamUpload_Request :
        ContractRequestBase,
        IServiceReturn<DamUpload_Response>
    {
        [Required]
        [ApiMember(Description = "File path without root folder", IsRequired = true)]
        public string Key { get; set; }

        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Not empty array", IsRequired = true)]
        public List<string> Tags { get; set; }

        [ApiMember(Description = "Key-Value metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        [ApiMember(Description = "Default: us-west-2 if unset")]
        public string Region { get; set; }

        [ApiMember(Description = "Default: Path.GetFileName(FileKey) if unset")]
        public string FileName { get; set; } // DisplayName

        [ApiMember(Description = "Default: Access level if public")]
        public bool? IsPublic { get; set; }

        [ApiMember(Description = "Parent resource id (if exists)")]
        public string ParentObjId { get; set; }

        public string Description { get; set; }

        public string Bucket { get; set; }
    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class DamUpload_Response :
        ContractResponseBase<ResListResource_ResponseModelItem>
    {

    }
    #endregion
}
