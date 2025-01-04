using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Config.Refresh
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    //[ContractRequired_ApiKey]
    [Api("Configserver:Refresh")]
    [Route("/refresh", "GET,POST")]
    [Route("/config/refresh", "GET,POST", Notes = "Bridge to ConfigServer V2")]
    [Route("/configserver/refresh", "GET,POST", Notes = "Bridge to ConfigServer V1")]
    public class LocConfigRefresh_Request :
        ContractRequestBase<LocConfigRefresh_RequestModel>,
        IServiceReturn<LocConfigRefresh_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LocConfigRefresh_Response :
        ContractResponseBase<LocConfigRefresh_ResponseModel>
    {

    }
    #endregion
}
