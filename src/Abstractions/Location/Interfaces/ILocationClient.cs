using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location.Enums;

namespace Nwpie.Foundation.Abstractions.Location.Interfaces
{
    public interface ILocationClient : IRefreshable, IDisposable
    {
        Task<string> GetApiUri(AppNameEnum appName);
        Task<string> GetApiLocation(string apiNameWithoutEnv);
        IDictionary<string, List<string>> GetAllLocationConfig();
        void RefreshAppEnvIpMapping();
    }
}
