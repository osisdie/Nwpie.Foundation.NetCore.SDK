using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation.Services
{
    public class LocGetApiLocation_DomainService :
    DomainService,
    ILocGetApiLocation_DomainService
    {
        public async Task<string> Execute(LocGetApiLocation_Request param)
        {
            var result = LocationHost.Instance
               .GetApiLocation(param)
               ?.Data;

            await Task.CompletedTask;
            return result;
        }
    }

}
