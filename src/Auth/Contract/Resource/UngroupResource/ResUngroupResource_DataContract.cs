using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.UngroupResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Unlink resource groups (parents/childs)")]
    [Route("/Resource/Ungrouping", "POST")]
    public class ResUngroupResource_Request :
        ContractRequestBase<ResUngroupResource_RequestModel>,
        IServiceReturn<ResUngroupResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResUngroupResource_Response :
        ContractResponseBase<ResUngroupResource_ResponseModel>
    {

    }
    #endregion
}
