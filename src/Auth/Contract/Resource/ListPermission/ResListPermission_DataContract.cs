using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListPermission
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("List user permissions")]
    [Route("/Resource/ListPermission", "GET,POST")]
    public class ResListPermission_Request :
        ContractRequestBase<ResListPermission_RequestModel>,
        IServiceReturn<ResListPermission_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResListPermission_Response :
        ContractResponseBase<ResListPermission_ResponseModel>
    {

    }
    #endregion
}
