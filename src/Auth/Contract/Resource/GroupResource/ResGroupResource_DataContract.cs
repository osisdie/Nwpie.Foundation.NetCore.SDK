using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Resource.GroupResource
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Link resource groups (parents/childs)")]
    [Route("/Resource/Grouping", "POST")]
    public class ResGroupResource_Request :
        ContractRequestBase<ResGroupResource_RequestModel>,
        IServiceReturn<ResGroupResource_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ResGroupResource_Response :
        ContractResponseBase<ResGroupResource_ResponseModel>
    {

    }
    #endregion
}
