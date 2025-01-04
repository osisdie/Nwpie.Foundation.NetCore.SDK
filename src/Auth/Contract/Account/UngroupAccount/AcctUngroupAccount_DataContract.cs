using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.UngroupAccount
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Unlink account groups (parents/childs)")]
    [Route("/Account/Ungrouping", "POST")]
    public class AcctUngroupAccount_Request :
        ContractRequestBase<AcctUngroupAccount_RequestModel>,
        IServiceReturn<AcctUngroupAccount_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class AcctUngroupAccount_Response :
        ContractResponseBase<AcctUngroupAccount_ResponseModel>
    {

    }
    #endregion
}
