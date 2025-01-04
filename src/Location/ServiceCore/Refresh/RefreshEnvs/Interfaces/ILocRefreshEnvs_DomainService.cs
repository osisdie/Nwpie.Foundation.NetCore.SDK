using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshEnvs.Interfaces
{
    public interface ILocRefreshEnvs_DomainService : IDomainService
    {
        Task<IDictionary<string, ServiceEnvironment>> Execute(LocRefreshEnvs_Request param);
    }
}
