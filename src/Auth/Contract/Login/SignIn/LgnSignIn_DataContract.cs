using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Login.SignIn
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Login for web")]
    [Route("/SignIn", "POST")]
    public class LgnSignIn_Request :
        ContractRequestBase<LgnSignIn_RequestModel>,
        IServiceReturn<LgnSignIn_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LgnSignIn_Response :
        ContractResponseBase<LgnSignIn_ResponseModel>
    {

    }
    #endregion
}
