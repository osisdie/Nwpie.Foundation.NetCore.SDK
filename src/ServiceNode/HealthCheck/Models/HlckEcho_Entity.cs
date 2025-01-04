using Nwpie.Foundation.Abstractions.DataAccess.Models;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Models
{
    public class HlckEcho_Entity : EntityBase
    {
        public HlckEcho_Entity() : base("") { }

        /// <summary>
        /// Response words. Say hello back
        /// </summary>
        [ApiMember(Description = "Response words. Say hello back")]
        public string words { get; set; }
    }
}
