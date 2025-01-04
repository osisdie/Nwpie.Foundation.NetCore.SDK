using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Configuration.SDK.Contracts.Set
{
    public class ConfigSet_ResponseModel : ResultDtoBase
    {
        public string VersionDisplay { get; set; } = string.Empty;
        public string VersionTimeStamp { get; set; } = string.Empty;
    }
}
