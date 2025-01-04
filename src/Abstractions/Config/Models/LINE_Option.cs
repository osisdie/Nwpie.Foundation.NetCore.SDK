namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class LINE_Option : OptionBase
    {
        public string HostUrl { get; set; }
        public string ApiKey { get; set; }
        public string Authorization { get; set; }
        public string DefaultRoom { get; set; }
        public string DefaultSender { get; set; }
    }
}
