using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.UptApplication
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Update the application")]
    [Route("/Application/Update", "POST")]
    public class AppUptApplication_Request :
        ContractRequestBase<AppUptApplication_RequestModel>,
        IServiceReturn<AppUptApplication_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AppUptApplication_Response :
        ContractResponseBase<AppUptApplication_ResponseModel>
    {

    }
    #endregion
}
