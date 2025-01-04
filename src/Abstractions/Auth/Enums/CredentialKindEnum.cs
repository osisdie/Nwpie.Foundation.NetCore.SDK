using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum CredentialKindEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "email")]
        Email = 1,

        [Display(Name = "mobile")]
        Mobile = 2,

        [Display(Name = "appname")]
        AppName = 3,

        [Display(Name = "apikey")]
        ApiKey = 11,

        [Display(Name = "PAT")]
        PAT = 12,

        Max = 255,
    }
}
