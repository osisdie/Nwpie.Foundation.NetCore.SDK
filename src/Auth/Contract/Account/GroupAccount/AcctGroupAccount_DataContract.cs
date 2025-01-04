using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.GroupAccount
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Link account groups (parents/childs)")]
    [Route("/Account/Grouping", "POST")]
    public class AcctGroupAccount_Request :
        ContractRequestBase<AcctGroupAccount_RequestModel>,
        IServiceReturn<AcctGroupAccount_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctGroupAccount_Response :
        ContractResponseBase<AcctGroupAccount_ResponseModel>
    {

    }
    #endregion
}
