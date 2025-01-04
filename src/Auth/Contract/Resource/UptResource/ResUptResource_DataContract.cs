using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.UptResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Update the resource")]
    [Route("/Resource/Update", "POST")]
    public class ResUptResource_Request :
        ContractRequestBase<ResUptResource_RequestModel>,
        IServiceReturn<ResUptResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResUptResource_Response :
        ContractResponseBase<ResUptResource_ResponseModel>
    {

    }
    #endregion
}
