using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation
{
    [CustomApiKeyFilter]
    public class LocGetApiLocation_Service : ApiServiceAnyInOutEntry<
    LocGetApiLocation_Request,
    string,
    ILocGetApiLocation_DomainService>
    {

    }
}
