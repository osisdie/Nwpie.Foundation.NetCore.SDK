namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class AwsSQS_Option : CloudProfile_Option
    {
        public string HomeArn { get; set; } // WithoutTopic
        public string QueueBaseUrl { get; set; }
        public string Topic { get; set; }
        public ushort? MaxConsumeCount { get; set; }
        public int? GetMessageTimeout { get; set; }
        public int? GetBatchTimeout { get; set; }
        public int? VisibilityTimeout { get; set; }
        public string QueueArn { get; set; } // Basically equals to {HomeArn}:{Topic}
        public string QueueUrl { get; set; } // Basically equals to {QueueBaseUrl}/{Topic}
        public string SubscriptionArn { get; set; } // The subscriptionArn subscripts to a specific location SNS
    }
}
