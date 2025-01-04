using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshAppEnvIpMapping.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh
{
    [CustomApiKeyFilter]
    public class LocRefreshAppEnvIpMapping_Service : ApiServiceAnyInOutEntry<
    LocRefreshAppEnvIpMapping_Request,
    bool,
    ILocRefreshAppEnvIpMapping_DomainService>
    {

    }
}
