using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.ServiceNode;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Contracts
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Echo example")]
    [Route("/" + SNConst.HealthCheckController + "/Echo", "GET,POST")]
    [Route("/" + SNConst.HealthCheckController + "/HlckEchoRequest", "GET,POST", Notes = "Obsolete")]
    public class HlckEcho_Request :
        ContractRequestBase<HlckEcho_RequestModel>,
        IServiceReturn<HlckEcho_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class HlckEcho_Response :
        ContractResponseBase<HlckEcho_ResponseModel>
    {

    }
    #endregion
}
