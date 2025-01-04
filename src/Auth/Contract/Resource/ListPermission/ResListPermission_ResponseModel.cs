using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListPermission
{
    public class ResListPermission_ResponseModel : ResultDtoBase<string>
    {
        public bool IsDevAdmin { get; set; }
    }
}
