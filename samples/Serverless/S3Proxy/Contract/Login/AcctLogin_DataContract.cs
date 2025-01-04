using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Contract.Login
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Login for web")]
    [Route("/Account/AcctLoginRequest", "POST")]
    public class AcctLogin_Request : ContractRequestBase<AcctLogin_RequestModel>, IServiceReturn<AcctLogin_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctLogin_Response : ContractResponseBase<AcctLogin_ResponseModel>
    {

    }
    #endregion
}
