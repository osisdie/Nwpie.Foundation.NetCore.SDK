using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.DeleteMe
{
    #region Data contract
    [Api("Delete account")]
    [Route("/Account/DeleteMe", "POST")]
    public class AcctDeleteMe_Request :
      ContractRequestBase<AcctDeleteMe_RequestModel>,
      IServiceReturn<AcctDeleteMe_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctDeleteMe_Response :
        ContractResponseBase<AcctDeleteMe_ResponseModel>
    {

    }
    #endregion
}
