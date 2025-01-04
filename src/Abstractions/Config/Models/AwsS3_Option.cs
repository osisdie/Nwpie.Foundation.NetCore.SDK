namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class AwsS3_Option : CloudProfile_Option
    {
        public string BucketName { get; set; }
        public bool CachePreSignedUrlEnabled { get; set; }
        public int CachePreSignedUrlMinutes { get; set; }
    }
}
