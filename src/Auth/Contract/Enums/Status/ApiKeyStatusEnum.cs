using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum ApiKeyStatusEnum
    {
        [Display(Name = "apikey.status.active")]
        Active,

        [Display(Name = "apikey.status.inactive")]
        InActive,
    }
}
