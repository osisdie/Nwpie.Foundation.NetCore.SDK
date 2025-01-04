namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Crypto_Option : OptionBase
    {
        public string AesSalt { get; set; }
        public string AesKey { get; set; }
        public string AesIV { get; set; }
    }
}
