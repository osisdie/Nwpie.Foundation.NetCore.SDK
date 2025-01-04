using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.HealthCheck.Echo
{
    public class HlckEcho_ResponseModel : ResultDtoBase
    {
        /// <summary>
        /// Echo Response (Includes request words)
        /// </summary>
        public string ResponseString { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; }
    }
}
