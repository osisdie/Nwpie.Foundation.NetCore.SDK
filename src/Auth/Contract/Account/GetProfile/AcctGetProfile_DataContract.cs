using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.GetProfile
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Get account's profile (after login)")]
    [Route("/Account/GetProfile", "GET,POST")]
    public class AcctGetProfile_Request :
        ContractRequestBase<AcctGetProfile_RequestModel>,
        IServiceReturn<AcctGetProfile_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctGetProfile_Response :
        ContractResponseBase<AcctGetProfile_ResponseModel>
    {

    }
    #endregion
}
