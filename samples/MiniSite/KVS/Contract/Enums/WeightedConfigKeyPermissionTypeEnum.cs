using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.KVS.Contract.Enums
{
    public enum WeightedConfigKeyPermissionTypeEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        SelfApp = 1,
        FriendApp = 2,
        PlatformAppInDebug = 98,
        PlatformApp = 99,
    }
}
