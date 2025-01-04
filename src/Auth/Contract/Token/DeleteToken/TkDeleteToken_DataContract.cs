using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.DeleteToken
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Delete the token for the account")]
    [Route("/Token/Remove", "POST")]
    public class TkDeleteToken_Request :
        ContractRequestBase<TkDeleteToken_RequestModel>,
        IServiceReturn<TkDeleteToken_Response>
    {
    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class TkDeleteToken_Response :
        ContractResponseBase<TkDeleteToken_ResponseModel>
    {

    }
    #endregion
}
