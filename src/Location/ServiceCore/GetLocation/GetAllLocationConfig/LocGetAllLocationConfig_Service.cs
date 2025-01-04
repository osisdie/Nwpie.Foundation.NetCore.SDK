using System.Collections.Generic;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.GetAllLocationConfig.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation
{
    [CustomApiKeyFilter]
    public class LocGetAllLocationConfig_Service : ApiServiceAnyInOutEntry<
    LocGetAllLocationConfig_Request,
    IDictionary<string, List<string>>,
    ILocGetAllLocationConfig_DomainService>
    {

    }
}
