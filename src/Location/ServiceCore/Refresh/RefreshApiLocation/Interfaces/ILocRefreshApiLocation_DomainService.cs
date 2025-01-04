using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshApiLocation.Interfaces
{
    public interface ILocRefreshApiLocation_DomainService : IDomainService
    {
        Task<bool> Execute(LocRefresh_Request param);
    }
}
