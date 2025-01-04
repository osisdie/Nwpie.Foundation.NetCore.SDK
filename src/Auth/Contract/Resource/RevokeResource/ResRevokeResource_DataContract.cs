using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.RevokeResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Disallow user access the resource")]
    [Route("/Resource/Revoke", "POST")]
    public class ResRevokeResource_Request :
        ContractRequestBase<ResRevokeResource_RequestModel>,
        IServiceReturn<ResRevokeResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResRevokeResource_Response :
        ContractResponseBase<ResRevokeResource_ResponseModel>
    {

    }
    #endregion
}
