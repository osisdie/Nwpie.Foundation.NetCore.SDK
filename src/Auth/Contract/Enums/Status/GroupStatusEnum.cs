using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum GroupStatusEnum
    {
        [Display(Name = "group.status.active")]
        Active,

        [Display(Name = "group.status.inactive")]
        InActive,
    }
}
