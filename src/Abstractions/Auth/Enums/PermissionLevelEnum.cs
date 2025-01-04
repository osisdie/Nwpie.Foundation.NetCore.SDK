using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum PermissionLevelEnum
    {
        [Display(Name = "none")]
        None = 0,

        [Display(Name = "login")]
        Login = 1,

        [Display(Name = "access")]
        Access = 2, // view

        [Display(Name = "query")]
        Query = 3,

        [Display(Name = "list")]
        QueryList = 4,

        [Display(Name = "download")]
        Download = 5,

        [Display(Name = "create")]
        Create = 11,

        [Display(Name = "upload")]
        Upload = 12,

        [Display(Name = "update")]
        Update = 21, // edit

        [Display(Name = "delete")]
        Delete = 31,

        [Display(Name = "full")]
        Full = 41,

        [Display(Name = "owner")]
        Owner = 51,

        [Display(Name = "devadmin")]
        DevAdmin = 99,

        [Display(Name = "deny")]
        Deny = 255,
    }
}
