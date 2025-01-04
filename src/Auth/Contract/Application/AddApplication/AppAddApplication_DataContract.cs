using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.AddApplication
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create top-level application")]
    [Route("/Application/Create", "POST")]
    public class AppAddApplication_Request :
        ContractRequestBase<AppAddApplication_RequestModel>,
        IServiceReturn<AppAddApplication_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AppAddApplication_Response :
        ContractResponseBase<AppAddApplication_ResponseModel>
    {

    }
    #endregion
}
