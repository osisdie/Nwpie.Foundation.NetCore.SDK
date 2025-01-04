namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class LocalStorage_Option : OptionBase
    {
        public string BasePath { get; set; }
        public string BucketName { get; set; } // Default folder
        public string User { get; set; }
        public string Pass { get; set; }
        public bool CachePreSignedUrlEnabled { get; set; }
        public int CachePreSignedUrlMinutes { get; set; }
    }
}
