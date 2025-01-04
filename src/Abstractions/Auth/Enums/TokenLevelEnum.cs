using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum TokenLevelEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "User")]
        EndUser = 1,
        [Display(Name = "PowerUser")]
        EndPowerUser = 2,
        [Display(Name = "Admininistrator")]
        EndAdminUser = 3,

        [Display(Name = "User Group")]
        EndUserGroup = 11,
        [Display(Name = "PowerUser Group")]
        EndPowerUserGroup = 12,
        [Display(Name = "Admininistrator Group")]
        EndAdminUserGroup = 13,

        [Display(Name = "Business User")]
        BusinessUser = 21,
        [Display(Name = "Business PowerUser")]
        BusinessPowerUser = 22,
        [Display(Name = "Business Admininistrator")]
        BusinessAdminUser = 23,

        [Display(Name = "Application User")]
        ApplicationUser = 91,
        [Display(Name = "Application PowerUser")]
        ApplicationPowerUser = 92,
        [Display(Name = "Application Admininistrator")]
        ApplicationAdminUser = 93,

        [Display(Name = "Platform User")]
        PlatformUser = 191,
        [Display(Name = "Platform PowerUser")]
        PlatformPowerUser = 192,
        [Display(Name = "Platform Admininistrator")]
        PlatformAdminUser = 193,

        Max = 255,
    }
}
