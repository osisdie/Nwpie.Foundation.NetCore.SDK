using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Resource.HasPermission
{
    public class ResHasPermission_ResponseModel : ResultDtoBase
    {
        public bool HasPermission { get; set; }
    }
}
