namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Smtp_Option : OptionBase
    {
        public string Host { get; set; }
        public int? Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public bool? EnableSSL { get; set; }
        public string BodyTemplate { get; set; }
    }
}
