using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.BatchGetApiLocations.Interfaces
{
    public interface ILocBatchGetApiLocations_DomainService : IDomainService
    {
        Task<IDictionary<string, string>> Execute(LocBatchGetApiLocations_Request param);
    }
}
