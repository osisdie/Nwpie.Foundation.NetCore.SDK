namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Measurement_Option : OptionBase
    {
        public bool? MeasurementEnabled { get; set; }
        public bool? RemoteLoggingEnabled { get; set; }
        public bool? CollectUsageMetricEnabled { get; set; }
        public string LocalLoggingFile { get; set; }
    }
}
