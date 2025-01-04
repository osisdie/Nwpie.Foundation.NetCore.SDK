using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.GetAllLocationConfig.Interfaces
{
    public interface ILocGetAllLocationConfig_DomainService : IDomainService
    {
        Task<IDictionary<string, List<string>>> Execute(LocGetAllLocationConfig_Request param);
    }
}
