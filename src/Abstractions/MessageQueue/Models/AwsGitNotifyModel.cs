using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.MessageQueue.Models
{
    public class AwsGitNotifyModel
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Subject { get; set; }
        public AwsGitMessage Message { get; set; }
        public DateTime? Timestamp { get; set; }
        public string SignatureVersion { get; set; }
        public string Signature { get; set; }
        public string SigningCertURL { get; set; }
        public string UnsubscribeURL { get; set; }
    }

    public class AwsGitMessage
    {
        public List<AwsGitRecord> Records { get; set; }
    }

    public class AwsGitReference
    {
        public string Commit { get; set; }
        public string Ref { get; set; }
    }

    public class AwsGitCodecommit
    {
        public List<AwsGitReference> References { get; set; }
    }

    public class AwsGitRecord
    {
        public string AwsRegion { get; set; }
        public AwsGitCodecommit Codecommit { get; set; }
        public string CustomData { get; set; }
        public string EventId { get; set; }
        public string EventName { get; set; }
        public int? EventPartNumber { get; set; }
        public string EventSource { get; set; }
        public string EventSourceARN { get; set; }
        public DateTime? EventTime { get; set; }
        public int? EventTotalParts { get; set; }
        public string EventTriggerConfigId { get; set; }
        public string EventTriggerName { get; set; }
        public string EventVersion { get; set; }
        public string UserIdentityARN { get; set; }
    }
}
