using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshApiLocation.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh
{
    [CustomApiKeyFilter]
    public class LocRefreshApiLocation_Service : ApiServiceAnyInOutEntry<
    LocRefresh_Request,
    bool,
    ILocRefreshApiLocation_DomainService>
    {

    }
}
