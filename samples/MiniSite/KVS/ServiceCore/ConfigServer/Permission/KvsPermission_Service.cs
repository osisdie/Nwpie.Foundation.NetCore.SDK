using Nwpie.MiniSite.KVS.Common.Attributes;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Permission;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission
{
    [CustomApiKeyFilter]
    public class KvsPermission_Service : CustomServiceEntryBase<
       KvsPermission_Request,
       KvsPermission_Response,
       KvsPermission_RequestModel,
       KvsPermission_ResponseModel,
       KvsPermission_ParamModel,
       IKvsPermission_DomainService>
    {
    }
}
