using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Location.Enums
{
    public enum AppNameEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "log")]
        LogSvc,

        [Display(Name = "loc")]
        LocationSvc,

        [Display(Name = "metric")]
        MeasurementSvc,

        [Display(Name = "cache")]
        CacheSvc,

        [Display(Name = "apistore")]
        ApiStore,

        [Display(Name = "config")]
        ConfigSvc,

        [Display(Name = "es")]
        ElasticsearchSvc,

        [Display(Name = "auth")]
        AuthSvc,

        [Display(Name = "storage")]
        StorageSvc,

        [Display(Name = "file")]
        FileSvc,

        [Display(Name = "hub")]
        NotificationSvc,
    }
}
