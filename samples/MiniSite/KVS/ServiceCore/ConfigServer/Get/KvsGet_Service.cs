using Nwpie.MiniSite.KVS.Common.Attributes;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get
{
    [CustomApiKeyFilter]
    public class KvsGet_Service : CustomServiceEntryBase<
       KvsGet_Request,
       KvsGet_Response,
       KvsGet_RequestModel,
       KvsGet_ResponseModel,
       KvsGet_ParamModel,
       IKvsGet_DomainService>
    {
    }
}
