using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.BatchGetApiLocations.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.BatchGetApiLocations.Services
{
    public class LocBatchGetApiLocations_DomainService :
    DomainService,
    ILocBatchGetApiLocations_DomainService
    {
        public async Task<IDictionary<string, string>> Execute(LocBatchGetApiLocations_Request param)
        {
            var result = LocationHost.Instance
                .BatchGetApiLocations(param)
                ?.Data;

            await Task.CompletedTask;
            return result;
        }
    }
}
