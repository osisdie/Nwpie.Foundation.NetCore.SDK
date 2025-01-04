using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshAppEnvIpMapping.Interfaces
{
    public interface ILocRefreshAppEnvIpMapping_DomainService : IDomainService
    {
        Task<bool> Execute(LocRefreshAppEnvIpMapping_Request param);
    }
}
