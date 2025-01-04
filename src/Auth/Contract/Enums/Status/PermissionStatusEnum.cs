using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum PermissionStatusEnum
    {
        [Display(Name = "permission.status.active")]
        Active,

        [Display(Name = "permission.status.inactive")]
        InActive,
    }
}
