namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class MessageQueue_OptionCollection : OptionBase
    {
        public AwsSQS_Option SQS { get; set; }
    }
}
