using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.RenewToken
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Renew existing access token by given refresh token, Jwt encrypted")]
    [Route("/Token/Renew", "POST")]
    [Route("/Auth/RenewToken", "POST")]
    [Route("/Renew/AccessToken", "POST")]
    public class TkRenewToken_Request :
        ContractRequestBase<TkRenewToken_RequestModel>,
        IServiceReturn<TkRenewToken_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class TkRenewToken_Response :
        ContractResponseBase<TkRenewToken_ResponseModel>
    {

    }
    #endregion
}
