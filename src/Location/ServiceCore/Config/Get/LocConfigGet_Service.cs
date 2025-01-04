using Nwpie.Foundation.Location.Contract.Config.Get;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.Config.Get.Interfaces;
using Nwpie.Foundation.Location.ServiceCore.Config.Get.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Get
{
    [CustomApiKeyFilter]
    public class LocConfigGet_Service : ApiServiceEntry<
    LocConfigGet_Request,
    LocConfigGet_Response,
    LocConfigGet_RequestModel,
    LocConfigGet_ResponseModel,
    LocConfigGet_ParamModel,
    ILocConfigGet_DomainService>
    {
    }
}
