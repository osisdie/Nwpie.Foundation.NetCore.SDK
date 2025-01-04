using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ResetPassword
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Reset password by manager")]
    [Route("/Account/ResetPassword", "POST")]
    public class AcctResetPassword_Request :
        ContractRequestBase<AcctResetPassword_RequestModel>,
        IServiceReturn<AcctResetPassword_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctResetPassword_Response :
        ContractResponseBase<AcctResetPassword_ResponseModel>
    {

    }
    #endregion
}
