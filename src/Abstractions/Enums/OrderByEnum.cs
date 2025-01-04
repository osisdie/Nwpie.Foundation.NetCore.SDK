using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Enums
{
    public enum OrderByEnum
    {
        [Display(Name = "ASC")]
        Ascending,

        [Display(Name = "DESC")]
        Descending
    }
}
