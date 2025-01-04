using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.UptLoginEmail
{
    #region Data contract

    [Api("Update account's login email")]
    [Route("/Account/UptLoginEmail", "POST")]
    public class AcctUptLoginEmail_Request :
        ContractRequestBase<AcctUptLoginEmail_RequestModel>,
        IServiceReturn<AcctUptLoginEmail_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctUptLoginEmail_Response :
        ContractResponseBase<AcctUptLoginEmail_ResponseModel>
    {

    }
    #endregion
}
