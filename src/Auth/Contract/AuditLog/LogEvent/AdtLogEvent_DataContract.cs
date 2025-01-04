using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.AuditLog.LogEvent
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Add event log")]
    [Route("/Log/Write", "POST")]
    public class AdtLogEvent_Request :
        ContractRequestBase<AdtLogEvent_RequestModel>,
        IServiceReturn<AdtLogEvent_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AdtLogEvent_Response :
        ContractResponseBase<AdtLogEvent_ResponseModel>
    {

    }
    #endregion
}
