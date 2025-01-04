using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Notification.Enums
{
    public enum NotifyChannelEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "smtp")]
        Email = 1,

        [Display(Name = "sms")]
        SMS = 2,

        [Display(Name = "line")]
        Line = 3,

        [Display(Name = "slack")]
        Slack = 4,

        [Display(Name = "webhook")]
        WebHook = 5,

        [Display(Name = "skype")]
        Skype = 6,

        [Display(Name = "teams")]
        Teams = 7,

        [Display(Name = "cloudwatch")]
        CloudWatch = 8,
    };
}
