using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum AccountStatusEnum
    {
        [Display(Name = "account.status.active")]
        Active,

        [Display(Name = "account.status.inactive")]
        InActive,
    }
}
