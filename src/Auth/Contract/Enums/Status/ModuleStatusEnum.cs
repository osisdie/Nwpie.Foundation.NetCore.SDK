using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Status
{
    public enum ModuleStatusEnum
    {
        [Display(Name = "module.status.active")]
        Active,

        [Display(Name = "module.status.inactive")]
        InActive,
    }
}
