using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.ValidateToken
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Validate token, Jwt encrypted")]
    [Route("/Token/Validate", "POST")]
    [Route("/Auth/ValidateToken", "POST")]
    [Route("/Validate/Token", "POST")]
    public class TkValidateToken_Request :
        ContractRequestBase<TkValidateToken_RequestModel>,
        IServiceReturn<TkValidateToken_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class TkValidateToken_Response :
        ContractResponseBase<TkValidateToken_ResponseModel>
    {

    }
    #endregion
}
