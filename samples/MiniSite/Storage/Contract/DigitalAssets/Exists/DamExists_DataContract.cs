using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.Contract.Assets.Exists
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Storage:Check if the file exists")]
    [Route("/Exists", "GET")]
    public class DamExists_Request :
        ContractRequestBase,
        IServiceReturn<DamExists_Response>
    {
        [Required]
        [ApiMember(Description = "File path without bucket", IsRequired = true)]
        public string Key { get; set; }

        public string Bucket { get; set; }
    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class DamExists_Response :
        ContractResponseBase<ResListResource_ResponseModelItem>
    {

    }
    #endregion
}
