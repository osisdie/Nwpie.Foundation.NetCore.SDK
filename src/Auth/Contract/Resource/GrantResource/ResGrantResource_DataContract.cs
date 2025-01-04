using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.GrantResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Allow user access the resource")]
    [Route("/Resource/Grant", "POST")]
    public class ResGrantResource_Request :
        ContractRequestBase<ResGrantResource_RequestModel>,
        IServiceReturn<ResGrantResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResGrantResource_Response :
        ContractResponseBase<ResGrantResource_ResponseModel>
    {

    }
    #endregion
}
