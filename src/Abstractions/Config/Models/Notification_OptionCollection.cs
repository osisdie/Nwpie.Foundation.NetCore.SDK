namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Notification_OptionCollection : OptionBase
    {
        public SendEmail_Option Email { get; set; }
        public Smtp_Option Smtp { get; set; }
        public Sendgrid_Option Sendgrid { get; set; }
        public AwsSQS_Option Sqs { get; set; }
    }
}
