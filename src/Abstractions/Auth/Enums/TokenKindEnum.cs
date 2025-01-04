using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum TokenKindEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "access_token")]
        AccessToken = 1,

        [Display(Name = "refresh_token")]
        RefreshToken = 2,

        [Display(Name = "admin_token")]
        AdminToken = 3,

        Max = 255
    }
}
