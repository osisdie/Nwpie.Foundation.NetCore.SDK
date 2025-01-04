using Nwpie.MiniSite.KVS.Common.Attributes;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer
{
    [CustomApiKeyFilter]
    public class KvsSet_Service : CustomServiceEntryBase<
       KvsSet_Request,
       KvsSet_Response,
       KvsSet_RequestModel,
       KvsSet_ResponseModel,
       KvsSet_ParamModel,
       IKvsSet_DomainService>
    {
    }
}
