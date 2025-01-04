using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ForgetPassword
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Forget password (then send temp password to email)")]
    [Route("/Account/ForgetPassword", "POST")]
    public class AcctForgetPassword_Request :
        ContractRequestBase<AcctForgetPassword_RequestModel>,
        IServiceReturn<AcctForgetPassword_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctForgetPassword_Response :
        ContractResponseBase<AcctForgetPassword_ResponseModel>
    {

    }
    #endregion
}
