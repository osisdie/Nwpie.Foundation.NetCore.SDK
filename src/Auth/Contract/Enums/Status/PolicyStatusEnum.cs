using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum PolicyStatusEnum
    {
        [Display(Name = "policy.status.active")]
        Active,

        [Display(Name = "policy.status.inactive")]
        InActive,
    }
}
