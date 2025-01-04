using System.Threading.Tasks;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshAppEnvIpMapping.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshAppEnvIpMapping.Services
{
    public class LocRefreshAppEnvIpMapping_DomainService :
    DomainService,
    ILocRefreshAppEnvIpMapping_DomainService
    {
        public async Task<bool> Execute(LocRefreshAppEnvIpMapping_Request param)
        {
            LocationHost.Instance.LoadDataFromLastSyncTime();

            await Task.CompletedTask;
            return true;
        }
    }
}
