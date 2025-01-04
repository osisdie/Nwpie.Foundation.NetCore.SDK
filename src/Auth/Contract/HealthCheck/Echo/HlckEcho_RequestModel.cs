using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.HealthCheck.Echo
{
    public class HlckEcho_RequestModel : AuthParamModel // RequestDtoBase
    {
        [Required]
        [ApiMember(Description = "Echo words", IsRequired = true)]
        public string RequestString { get; set; }
    }
}
