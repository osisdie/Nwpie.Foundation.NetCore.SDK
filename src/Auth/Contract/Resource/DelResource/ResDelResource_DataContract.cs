using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.DelResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Delete the attached resource")]
    [Route("/Resource/Delete", "POST")]
    public class ResDelResource_Request :
        ContractRequestBase<ResDelResource_RequestModel>,
        IServiceReturn<ResDelResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResDelResource_Response :
        ContractResponseBase<ResDelResource_ResponseModel>
    {

    }
    #endregion
}
