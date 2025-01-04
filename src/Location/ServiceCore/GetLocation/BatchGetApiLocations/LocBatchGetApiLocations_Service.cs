using System.Collections.Generic;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.ServiceCore.Attributes;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.BatchGetApiLocations.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation
{
    [CustomApiKeyFilter]
    public class LocBatchGetApiLocations_Service : ApiServiceAnyInOutEntry<
    LocBatchGetApiLocations_Request,
    IDictionary<string, string>,
    ILocBatchGetApiLocations_DomainService>
    {

    }
}
