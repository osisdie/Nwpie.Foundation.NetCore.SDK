using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.SetDirectly
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    //[ContractRequired_ApiKey]
    [Api("Configserver:SetDirectly")]
    [Route("/configserver/setdirectly", "POST")]
    public class KvsSetDirectly_Request : IServiceReturn<object>// ContractRequestBase<KvsSet_RequestModel>, IServiceReturn<KvsSetDirectly_Response>
    {

    }

    #endregion
}
