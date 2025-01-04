using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Common.Notification.Models
{
    public static class MailConfig
    {
        public static string DisplayName
        {
            get => "Nwpie Tech";
        }

        public static string EmailFrom
        {
            get => "service@kevinw.net";
        }

        public static string SubjectPrefix
        {
            get => $"[{CommonConst.SdkPrefix}] ";
        }

        public static string Footer
        {
            get => $"<p style='MARGIN - BOTTOM: 0px; MAX - WIDTH: 100 %; COLOR: #aaa; TEXT-ALIGN: left; LINE-HEIGHT: 20px; FONT-SIZE:14px;'>Data Center<br />Nwpie Tech Inc.<br /><a href='https://about:blank'></a></p>";
        }
    }
}
