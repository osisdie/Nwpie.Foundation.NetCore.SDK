namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Slack_Option : OptionBase
    {
        public string WebHookUrl { get; set; }
        public string DefaultChannel { get; set; }
        public string DefaultSender { get; set; }
        public string Icon { get; set; }
    }
}
