using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Auth.Contract.Enums.Kind
{
    public enum AuthTagNameEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "email")]
        Email,

        [Display(Name = "mobile")]
        Mobile,

        [Display(Name = "appname")]
        AppName,

        [Display(Name = "apiname")]
        ApiName,

        [Display(Name = "PAT")]
        PAT,

        [Display(Name = "application")]
        Application,

        [Display(Name = "b2b_customer")]
        CustomerB2B,

        [Display(Name = "end_user")]
        EndUser,

        [Display(Name = "permission")]
        Permission,
    }
}
