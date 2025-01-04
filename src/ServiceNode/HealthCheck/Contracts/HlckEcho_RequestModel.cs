using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Contracts
{
    public class HlckEcho_RequestModel : RequestDtoBase
    {
        [Required]
        [ApiMember(Description = "Echo words", IsRequired = true)]
        public string RequestString { get; set; }
    }
}
