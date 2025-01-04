using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Kind
{
    public enum AuthModuleKindEnum
    {
        [Display(Name = "ltnl")]
        Internal,

        [Display(Name = "tk")]
        Token,

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
