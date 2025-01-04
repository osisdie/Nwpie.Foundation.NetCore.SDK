using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ChangePassword
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Change password (after login)")]
    [Route("/Account/ChangePassword", "POST")]
    public class AcctChangePassword_Request :
        ContractRequestBase<AcctChangePassword_RequestModel>,
        IServiceReturn<AcctChangePassword_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctChangePassword_Response :
        ContractResponseBase<AcctChangePassword_ResponseModel>
    {

    }
    #endregion
}
