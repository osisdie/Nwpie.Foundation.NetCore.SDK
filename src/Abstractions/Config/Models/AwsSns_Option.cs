using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class AwsSNS_Option : CloudProfile_Option
    {
        public string HomeArn { get; set; } // WithoutTopic
        public string Topic { get; set; }
        public string Protocol { get; set; }
        public int? MaxReceivesPerSecond { get; set; }
        public bool? RawMessageEnabled { get; set; }
        public List<string> SubscriptionArns { get; set; }
        public string TopicArn { get; set; } // Basically equals to {HomeArn}:{Topic}
    }
}
