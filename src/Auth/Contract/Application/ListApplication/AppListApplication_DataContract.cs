using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.ListApplication
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("List all applications")]
    [Route("/Application/List", "GET,POST")]
    public class AppListApplication_Request :
        ContractRequestBase<AppListApplication_RequestModel>,
        IServiceReturn<AppListApplication_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AppListApplication_Response :
        ContractResponseBase<AppListApplication_ResponseModel>
    {

    }
    #endregion
}
