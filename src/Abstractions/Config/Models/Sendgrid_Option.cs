namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Sendgrid_Option : OptionBase
    {
        public string Apikey { get; set; }
        public string BodyTemplate { get; set; }
    }
}
