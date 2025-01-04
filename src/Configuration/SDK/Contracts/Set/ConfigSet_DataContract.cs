using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Set
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Configserver:Set")]
    [Route(LocationConst.HttpPathToConfigContractRequest_Set, "POST")]
    public class ConfigSet_Request :
        ContractRequestBase<ConfigSet_RequestModel>,
        IServiceReturn<ConfigSet_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ConfigSet_Response :
        ContractResponseBase<ConfigSet_ResponseModel>
    {

    }
    #endregion
}
