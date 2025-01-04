using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Lookup.ListOptions
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Get Mapping Table")]
    [Route("/Lookup/ListOption", "GET,POST")]
    public class LkpListOptions_Request :
        ContractRequestBase<LkpListOptions_RequestModel>,
        IServiceReturn<LkpListOptions_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LkpListOptions_Response :
        ContractResponseBase<LkpListOptions_ResponseModel>
    {

    }
    #endregion
}
