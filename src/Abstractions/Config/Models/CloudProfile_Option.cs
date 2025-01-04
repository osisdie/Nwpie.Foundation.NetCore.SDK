namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class CloudProfile_Option : OptionBase
    {
        public string ServiceUrl { get; set; }
        public string ResourceId { get; set; }
        public string Region { get; set; }
        public string Profile { get; set; }
        public string CredentialPath { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
