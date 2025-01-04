using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.UptProfile
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Update account's profile")]
    [Route("/Account/UptProfile", "POST")]
    public class AcctUptProfile_Request :
        ContractRequestBase<AcctUptProfile_RequestModel>,
        IServiceReturn<AcctUptProfile_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctUptProfile_Response :
        ContractResponseBase<AcctUptProfile_ResponseModel>
    {

    }
    #endregion
}
