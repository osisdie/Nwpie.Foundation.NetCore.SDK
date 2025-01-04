using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.HealthCheck.Echo
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Echo example")]
    [Route("/HealthCheck/HlckEchoRequest", "GET,POST")]
    public class HlckEcho_Request : ContractRequestBase<HlckEcho_RequestModel>, IServiceReturn<HlckEcho_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class HlckEcho_Response : ContractResponseBase<HlckEcho_ResponseModel>
    {

    }
    #endregion
}
