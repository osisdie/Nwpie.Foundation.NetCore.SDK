using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.GetLocation.GetAllLocationConfig.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.GetAllLocationConfig.Services
{
    public class LocGetAllLocationConfig_DomainService :
    DomainService,
    ILocGetAllLocationConfig_DomainService
    {
        public async Task<IDictionary<string, List<string>>> Execute(LocGetAllLocationConfig_Request param)
        {
            var result = XMLReader.GetAllLocationConfig();

            await Task.CompletedTask;
            return result;
        }
    }
}
