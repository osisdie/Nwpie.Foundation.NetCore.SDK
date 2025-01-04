using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;

namespace Nwpie.Foundation.Abstractions.Config.Interfaces
{
    public interface IConfigClient : ISingleCObject
    {
        Task<IServiceResponse<string>> GetLatest(string configKey, string apiName = null, string apiKey = null);
        Task<IServiceResponse<T>> GetLatest<T>(string configKey, string apiName = null, string apiKey = null)
            where T : class;

        Task<IServiceResponse<Dictionary<string, string>>> GetLatest(List<ConfigItem> configs, string apiName = null, string apiKey = null);
        Task<IServiceResponse<bool>> Upsert(string configKey, string config, bool encrypt, string apiName = null, string apiKey = null);

        string GetUrl { get; set; }
        string SetUrl { get; set; }
        int DefaultTimeoutSecs { get; set; }
        int DefaultRetries { get; set; }
        int DefaultDelayRetrySecs { get; set; }
    }
}
