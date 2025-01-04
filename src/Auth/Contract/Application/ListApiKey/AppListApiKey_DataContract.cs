using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.ListApiKey
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("List all apikey in applications")]
    [Route("/Application/ListApiKey", "GET,POST")]
    public class AppListApiKey_Request :
        ContractRequestBase<AppListApiKey_RequestModel>,
        IServiceReturn<AppListApiKey_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AppListApiKey_Response :
        ContractResponseBase<AppListApiKey_ResponseModel>
    {

    }
    #endregion
}
