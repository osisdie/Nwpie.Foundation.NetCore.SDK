using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Get
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Configserver:Get")]
    [Route(LocationConst.HttpPathToConfigContractRequest_Get, "GET,POST")]
    public class ConfigGet_Request :
        ContractRequestBase<ConfigGet_RequestModel>,
        IServiceReturn<ConfigGet_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ConfigGet_Response :
        ContractResponseBase<ConfigGet_ResponseModel>
    {

    }
    #endregion
}
