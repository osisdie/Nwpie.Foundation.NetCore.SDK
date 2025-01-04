using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum JwtHashAlgorithmEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "default")]
        RS256 = 1,

        HS384 = 2,

        HS512 = 3,

        MAX = 255
    }
}
