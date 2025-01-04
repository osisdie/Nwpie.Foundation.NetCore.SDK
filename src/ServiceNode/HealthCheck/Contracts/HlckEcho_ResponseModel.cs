using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Contracts
{
    public class HlckEcho_ResponseModel : ResultDtoBase
    {
        public string ResponseString { get; set; }
        public string Version { get; set; }
    }
}
