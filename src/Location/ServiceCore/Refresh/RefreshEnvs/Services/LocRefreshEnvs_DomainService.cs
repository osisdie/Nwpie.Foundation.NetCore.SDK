using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshEnvs.Interfaces;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshEnvs.Services
{
    public class LocRefreshEnvs_DomainService :
    DomainService,
    ILocRefreshEnvs_DomainService
    {
        public async Task<IDictionary<string, ServiceEnvironment>> Execute(LocRefreshEnvs_Request param)
        {
            var result = XMLReader.RefreshEnvs();

            await Task.CompletedTask;
            return result;
        }
    }
}
