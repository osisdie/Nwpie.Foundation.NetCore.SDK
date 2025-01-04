using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Login.SignOut
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Logout")]
    [Route("/SignOut", "POST")]
    public class LgnSignOut_Request :
        ContractRequestBase<LgnSignOut_RequestModel>,
        IServiceReturn<LgnSignOut_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class LgnSignOut_Response :
        ContractResponseBase<LgnSignOut_ResponseModel>
    {

    }
    #endregion
}
