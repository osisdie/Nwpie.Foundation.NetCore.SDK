using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.DelApplication
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Remove the application")]
    [Route("/Application/Delete", "POST")]
    public class AppDelApplication_Request :
        ContractRequestBase<AppDelApplication_RequestModel>,
        IServiceReturn<AppDelApplication_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AppDelApplication_Response :
        ContractResponseBase<AppDelApplication_ResponseModel>
    {

    }
    #endregion
}
