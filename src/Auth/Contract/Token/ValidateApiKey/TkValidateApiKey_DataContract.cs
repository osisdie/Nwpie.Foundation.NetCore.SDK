using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Token.ValidateApiKey
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Validate ApiKey and response profile")]
    [Route("/Token/ValidateApiKey", "POST")]
    [Route("/Validate/ApiKey", "POST")]
    public class TkValidateApiKey_Request :
        ContractRequestBase<TkValidateApiKey_RequestModel>,
        IServiceReturn<TkValidateApiKey_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class TkValidateApiKey_Response :
        ContractResponseBase<TkValidateApiKey_ResponseModel>
    {

    }
    #endregion
}
