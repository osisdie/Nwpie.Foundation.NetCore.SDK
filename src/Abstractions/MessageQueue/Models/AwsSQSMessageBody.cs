using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Models
{
    public class AwsSQSMessageBody
    {
        public int DelaySeconds { get; set; }
        public Dictionary<string, MessageAttributeValue> MessageAttributes { get; set; }
        public string MessageBody { get; set; }
        public string MessageDeduplicationId { get; set; }
        public string MessageGroupId { get; set; }
        public string QueueUrl { get; set; }
    }

    public class MessageAttributeValue
    {
        public string DataType { get; set; }
        public List<string> StringListValues { get; set; }
        public string StringValue { get; set; }
    }
}
