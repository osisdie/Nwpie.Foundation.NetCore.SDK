using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.DataAccess.Enums
{
    public enum DataSourceEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "mysql")]
        MySQL = 1,

        [Display(Name = "aurora")]
        Aurora = 2,

        [Display(Name = "sqlserver")]
        SqlServer = 3,

        [Display(Name = "mongodb")]
        MongoDB = 11,

        [Display(Name = "dynamodb")]
        DynamoDB = 12,

        [Display(Name = "datastore")]
        DataStore = 13,

        [Display(Name = "file")]
        File = 21,

        [Display(Name = "ftp")]
        FTP = 22,

        [Display(Name = "s3")]
        S3 = 23,

        [Display(Name = "dropbox")]
        Dropbox = 24,

        [Display(Name = "googledrive")]
        GoogleDrive = 25,

        [Display(Name = "es")]
        ElasticSearch = 31,

        [Display(Name = "influxdb")]
        InfluxDB = 32,
    };
}
