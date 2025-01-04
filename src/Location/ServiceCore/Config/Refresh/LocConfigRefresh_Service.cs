using Nwpie.Foundation.Location.Contract.Config.Refresh;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Interfaces;
using Nwpie.Foundation.Location.ServiceCore.Config.Refresh.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Config.Refresh
{
    [CustomApiKeyFilter]
    public class LocConfigRefresh_Service : ApiServiceEntry<
    LocConfigRefresh_Request,
    LocConfigRefresh_Response,
    LocConfigRefresh_RequestModel,
    LocConfigRefresh_ResponseModel,
    LocConfigRefresh_ParamModel,
    ILocConfigRefresh_DomainService>
    {
    }
}
