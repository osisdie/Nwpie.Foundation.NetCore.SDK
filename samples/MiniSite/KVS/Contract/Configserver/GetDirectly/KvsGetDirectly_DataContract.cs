using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.GetDirectly
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    //[ContractRequired_ApiKey]
    [Api("Configserver:GetDirectly")]
    [Route("/configserver/getdirectly", "GET,POST")]
    public class KvsGetDirectly_Request : IServiceReturn<object>// ContractRequestBase<KvsGet_RequestModel>, IServiceReturn<KvsGetDirectly_Response>
    {

    }

    #endregion
}
