using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Logging.Enums
{
    public enum LoggingProviderEnum
    {
        [Display(Name = "unknown")]
        UnSet = 0,

        [Display(Name = "log4net")]
        Log4net,

        [Display(Name = "nlog")]
        NLog,

        [Display(Name = "es")]
        ElasticSearch,

        [Display(Name = "serilog")]
        Serilog
    }
}
