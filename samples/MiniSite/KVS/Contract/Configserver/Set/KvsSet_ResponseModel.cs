using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Set
{
    public class KvsSet_ResponseModel : ResultDtoBase
    {
        public string VersionDisplay { get; set; } = string.Empty;
        public string VersionTimeStamp { get; set; } = string.Empty;
    }
}
