namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class DataAccess_OptionCollection : OptionBase
    {
        public AwsS3_Option S3 { get; set; }
        public AwsRDS_Option Rds { get; set; }
    }
}
