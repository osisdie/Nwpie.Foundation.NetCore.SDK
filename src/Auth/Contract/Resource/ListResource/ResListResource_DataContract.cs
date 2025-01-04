using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("List all attached resources")]
    [Route("/Resource/List", "GET,POST")]
    public class ResListResource_Request :
        ContractRequestBase<ResListResource_RequestModel>,
        IServiceReturn<ResListResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResListResource_Response :
        ContractResponseBase<ResListResource_ResponseModel>
    {

    }
    #endregion
}
