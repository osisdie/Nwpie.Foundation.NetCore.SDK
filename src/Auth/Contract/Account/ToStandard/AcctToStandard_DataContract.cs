using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.ToStandard
{
    #region Data contract
    [Api("Convert account")]
    [Route("/Account/ToStandard", "POST")]
    public class AcctToStandard_Request :
      ContractRequestBase<AcctToStandard_RequestModel>,
      IServiceReturn<AcctToStandard_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctToStandard_Response :
        ContractResponseBase<AcctToStandard_ResponseModel>
    {

    }
    #endregion
}
