using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Enums
{
    public enum ModuleKindEnum
    {
        [Display(Name = "ltnl")]
        Internal,

        [Display(Name = "auth")]
        Auth,

        [Display(Name = "lgn")]
        Login,

        [Display(Name = "acct")]
        Account,

        [Display(Name = "app")]
        Application,

        [Display(Name = "mdl")]
        Module,

        [Display(Name = "grp")]
        Group,

        [Display(Name = "plcy")]
        Policy,

        [Display(Name = "perm")]
        Permission,

        [Display(Name = "adt")]
        AuditLog,

        [Display(Name = "lkp")]
        Lookup,

        [Display(Name = "sys")]
        System,

        [Display(Name = "hlck")]
        HealthCheck,
    }
}
