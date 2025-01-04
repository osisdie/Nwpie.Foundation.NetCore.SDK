using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum ResourceStatusEnum
    {
        [Display(Name = "resource.status.active")]
        Active,

        [Display(Name = "resource.status.inactive")]
        InActive,
    }
}
