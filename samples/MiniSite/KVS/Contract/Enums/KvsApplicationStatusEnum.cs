using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.KVS.Contract.Enums
{
    public enum KvsApplicationStatusEnum
    {
        [Display(Name = "account.status.active")]
        Active,

        [Display(Name = "account.status.inactive")]
        InActive,
    }
}
