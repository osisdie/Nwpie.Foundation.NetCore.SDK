using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshEnvs.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh
{
    [CustomApiKeyFilter]
    public class LocRefreshEnvs_Service : ApiServiceAnyInOutEntry<
    LocRefreshEnvs_Request,
    IDictionary<string, ServiceEnvironment>,
    ILocRefreshEnvs_DomainService>
    {

    }
}
