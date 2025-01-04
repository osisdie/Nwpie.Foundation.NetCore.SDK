using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.Contract.Assets.Search
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Asset:Search resources")]
    [Route("/Search", "GET,POST")]
    public class DamSearch_Request :
        ContractRequestBase<ResListResource_RequestModel>,
        IServiceReturn<DamSearch_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class DamSearch_Response :
        ContractResponseBase<ResListResource_ResponseModel>
    {

    }
    #endregion
}
