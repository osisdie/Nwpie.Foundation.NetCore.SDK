using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum ApplicationStatusEnum
    {
        [Display(Name = "account.status.active")]
        Active,

        [Display(Name = "account.status.inactive")]
        InActive,
    }
}
