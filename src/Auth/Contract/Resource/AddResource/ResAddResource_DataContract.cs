using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.AddResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new resource")]
    [Route("/Resource/Create", "POST")]
    public class ResAddResource_Request :
        ContractRequestBase<ResAddResource_RequestModel>,
        IServiceReturn<ResAddResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResAddResource_Response :
        ContractResponseBase<ResAddResource_ResponseModel>
    {

    }
    #endregion
}
