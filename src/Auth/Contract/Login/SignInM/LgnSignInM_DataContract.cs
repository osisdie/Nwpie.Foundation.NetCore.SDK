using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Login.SignInM
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Login for mobile App")]
    [Route("/m/SignIn", "POST")]
    public class LgnSignInM_Request :
        ContractRequestBase<LgnSignInM_RequestModel>,
        IServiceReturn<LgnSignInM_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LgnSignInM_Response :
        ContractResponseBase<LgnSignInM_ResponseModel>
    {

    }
    #endregion
}
