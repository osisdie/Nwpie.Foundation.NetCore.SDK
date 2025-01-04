using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Set
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    //[ContractRequired_ApiKey]
    //[ContractRouting_Http("/configserver/set", EMHttpVerbs.Post)]
    [Api("Configserver:Set")]
    [Route(LocationConst.HttpPathToConfigContractRequest_Set, "POST")]
    public class KvsSet_Request :
        ContractRequestBase<KvsSet_RequestModel>,
        IServiceReturn<KvsSet_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class KvsSet_Response :
        ContractResponseBase<KvsSet_ResponseModel>
    {

    }
    #endregion
}
