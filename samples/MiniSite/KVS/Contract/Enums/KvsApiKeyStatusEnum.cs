using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.KVS.Contract.Enums
{
    public enum KvsApiKeyStatusEnum
    {
        [Display(Name = "apikey.status.active")]
        Active,

        [Display(Name = "apikey.status.inactive")]
        InActive,
    }
}
