using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Config.Get
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Configserver:Get")]
    [Route("/get", "GET,POST")]
    [Route("/config/get", "GET,POST", Notes = "Bridge to ConfigServer V2")]
    [Route("/configserver/get", "GET,POST", Notes = "Bridge to ConfigServer V1")]
    public class LocConfigGet_Request :
        ContractRequestBase<LocConfigGet_RequestModel>,
        IServiceReturn<LocConfigGet_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LocConfigGet_Response :
        ContractResponseBase<LocConfigGet_ResponseModel>
    {

    }
    #endregion
}
