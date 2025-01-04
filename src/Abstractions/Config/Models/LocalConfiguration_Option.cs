namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class LocalConfiguration_Option : Measurement_Option
    {
        public bool? Enabled { get; set; }
        public string BasePath { get; set; }
        public string LocalConfigFilePath { get; set; }  // relative from BasePath folder
    }
}
