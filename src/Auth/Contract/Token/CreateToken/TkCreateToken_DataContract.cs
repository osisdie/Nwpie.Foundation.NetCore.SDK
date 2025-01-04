using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.CreateToken
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new token for the account")]
    [Route("/Token/CreateApiKeyPair", "POST")]
    public class TkCreateApiKeyPair_Request :
        ContractRequestBase<TkCreateApiKey_RequestModel>,
        IServiceReturn<TkCreateToken_Response>
    {
    }

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new token for the account")]
    [Route("/Token/CreatePAT", "POST")]
    public class TkCreatePAT_Request :
        ContractRequestBase<TkCreatePAT_RequestModel>,
        IServiceReturn<TkCreateToken_Response>
    {
    }

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create bearer token for the account")]
    [Route("/Token/CreateBearer", "POST")]
    public class TkCreateBearer_Request :
        ContractRequestBase<TkCreateBearer_RequestModel>,
        IServiceReturn<TkCreateToken_Response>
    {
    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class TkCreateToken_Response :
        ContractResponseBase<TkCreateToken_ResponseModel>
    {

    }
    #endregion
}
