using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ListAccount
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("List all valid accounts")]
    [Route("/Account/List", "GET,POST")]
    public class AcctListAccount_Request :
        ContractRequestBase<AcctListAccount_RequestModel>,
        IServiceReturn<AcctListAccount_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctListAccount_Response :
        ContractResponseBase<AcctListAccount_ResponseModel>
    {

    }
    #endregion
}
