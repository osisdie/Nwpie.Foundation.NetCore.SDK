using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.RecoveryPassword
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Recovery password (after forget)")]
    [Route("/Account/RecoveryPassword", "POST")]
    public class AcctRecoveryPassword_Request :
        ContractRequestBase<AcctRecoveryPassword_RequestModel>,
        IServiceReturn<AcctRecoveryPassword_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctRecoveryPassword_Response :
        ContractResponseBase<AcctRecoveryPassword_ResponseModel>
    {

    }
    #endregion
}
