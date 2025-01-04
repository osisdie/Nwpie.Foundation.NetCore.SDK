using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum PermissionTypeEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "app_id")]
        DenyAppId = -1,

        [Display(Name = "apikey")]
        DenyApiKey = -2,

        [Display(Name = "group_id")]
        DenyGroupId = -3,

        [Display(Name = "account_id")]
        DenyAccountId = -4,

        [Display(Name = "app_id")]
        AllowAppId = 1,

        [Display(Name = "apikey")]
        AllowApiKey = 2,

        [Display(Name = "group_id")]
        AllowGroupId = 3,

        [Display(Name = "account_id")]
        AllowAccountId = 4
    };

}
