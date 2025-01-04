namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class CORS_Option : OptionBase
    {
        public string AllowOrigin { get; set; }
        public string AllowMethods { get; set; }
        public string AllowHeaders { get; set; }
        public string DenyOrigin { get; set; }
    }
}
