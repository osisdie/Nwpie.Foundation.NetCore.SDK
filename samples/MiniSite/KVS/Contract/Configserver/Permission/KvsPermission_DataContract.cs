using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Permission
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    //[ContractRequired_ApiKey]
    //[ContractRouting_Http("/configserver/permission/modify", EMHttpVerbs.Post)]
    [Api("Configserver:Grant Permission")]
    [Route("/configserver/permission/modify", "POST")]
    public class KvsPermission_Request : ContractRequestBase<KvsPermission_RequestModel>, IServiceReturn<KvsPermission_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class KvsPermission_Response : ContractResponseBase<KvsPermission_ResponseModel>
    {

    }
    #endregion
}
