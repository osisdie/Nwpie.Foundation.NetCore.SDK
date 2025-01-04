using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Get
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Configserver:Get")]
    [Route(LocationConst.HttpPathToConfigContractRequest_Get, "GET,POST")]
    public class KvsGet_Request : ContractRequestBase<KvsGet_RequestModel>, IServiceReturn<KvsGet_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class KvsGet_Response : ContractResponseBase<KvsGet_ResponseModel>
    {

    }
    #endregion
}
