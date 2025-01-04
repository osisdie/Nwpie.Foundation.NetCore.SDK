namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class RabbitMQ_Option : OptionBase
    {
        public string Host { get; set; }
        public string VHost { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public ushort? MaxConsumeCount { get; set; }
        public int? GetMessageTimeout { get; set; } // ms
        public int? GetBatchTimeout { get; set; } // ms
    }
}
