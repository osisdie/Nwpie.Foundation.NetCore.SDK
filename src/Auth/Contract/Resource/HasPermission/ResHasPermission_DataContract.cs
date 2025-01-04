using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.HasPermission
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Has specific permission")]
    [Route("/Resource/HasPermission", "POST")]
    public class ResHasPermission_Request :
        ContractRequestBase<ResHasPermission_RequestModel>,
        IServiceReturn<ResHasPermission_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResHasPermission_Response :
        ContractResponseBase<ResHasPermission_ResponseModel>
    {

    }
    #endregion
}
