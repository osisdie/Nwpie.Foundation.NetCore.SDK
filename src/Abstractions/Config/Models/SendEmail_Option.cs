namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class SendEmail_Option : OptionBase
    {
        public string Sender { get; set; }
        public string Mailfrom { get; set; }
        public string Mailto { get; set; }
        public string TitlePrefix { get; set; }
        public string BodyPostfix { get; set; }
    }
}
