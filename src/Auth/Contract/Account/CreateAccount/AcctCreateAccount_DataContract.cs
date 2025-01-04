using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.CreateAccount
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create device account via email")]
    [Route("/Account/CreateByDevice", "POST")]
    public class AcctCreateDeviceAccountByEmail_Request :
        ContractRequestBase<AcctCreateAccountByDevice_RequestModel>,
        IServiceReturn<AcctCreateAccount_Response>
    { }

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new account via email")]
    [Route("/Account/CreateByEmail", "POST")]
    public class AcctCreateAccountByEmail_Request :
        ContractRequestBase<AcctCreateAccountByEmail_RequestModel>,
        IServiceReturn<AcctCreateAccount_Response>
    { }

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new account via mobile phone")]
    [Route("/Account/CreateByMobile", "POST")]
    public class AcctCreateAccountByMobile_Request :
        ContractRequestBase<AcctCreateAccountByMobile_RequestModel>,
        IServiceReturn<AcctCreateAccount_Response>
    { }

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Create new account via application name")]
    [Route("/Account/CreateByAppName", "POST")]
    public class AcctCreateAccountByAppName_Request :
        ContractRequestBase<AcctCreateAccountByAppName_RequestModel>,
        IServiceReturn<AcctCreateAccount_Response>
    { }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctCreateAccount_Response :
        ContractResponseBase<AcctCreateAccount_ResponseModel>
    {

    }
    #endregion
}
