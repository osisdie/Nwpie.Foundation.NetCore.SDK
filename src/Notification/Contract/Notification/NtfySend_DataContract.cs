using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using ServiceStack;

namespace Nwpie.Foundation.Notification.Contract.Notification
{
    #region Data contract
    [Api("Send notification to Email, Line or Slack")]
    [Route("/Notification/Send", "GET,POST")]
    public class NtfySend_Request :
        ContractRequestBase<NotifySend_RequestModel>,
        IServiceReturn<NtfySend_Response>
    {

    }

    public class NtfySend_Response :
        ContractResponseBase<ResultDtoBase> // NO DATA
    {

    }
    #endregion
}
