using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Enums
{
    /// <summary>
    /// @See EMConfiguration
    /// </summary>
    public enum EnvironmentEnum
    {
        [Display(Name = "base")]
        Testing = 0,

        [Display(Name = "debug")]
        Debug = 1,

        [Display(Name = "dev")]
        Development = 2,

        [Display(Name = "stage")]
        Staging = 3,

        [Display(Name = "preprod")]
        Staging_2 = 4,

        [Display(Name = "prod")]
        Production = 5,

        [Display(Name = "unknown")]
        Max = 255
    }
}
