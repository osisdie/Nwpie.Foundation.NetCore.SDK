using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.GetLocation.GetApiLocation.Interfaces
{
    public interface ILocGetApiLocation_DomainService : IDomainService
    {
        Task<string> Execute(LocGetApiLocation_Request param);
    }
}
