using System;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Common.MessageQueue.Models
{
    public class GcpGitMessageModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime EventTime { get; set; }
        public GcpRefUpdateEvent RefUpdateEvent { get; set; }
    }

    public class GcpRefsHeads
    {
        public string RefName { get; set; }
        public string UpdateType { get; set; }
        public string OldId { get; set; }
        public string NewId { get; set; }
    }

    public class GcpRefUpdates
    {
        [JsonProperty(PropertyName = "refs/heads/dev")]
        public GcpRefsHeads DevUpdates { get; set; }

        [JsonProperty(PropertyName = "refs/heads/master")]
        public GcpRefsHeads MasterUpdates { get; set; }
    }

    public class GcpRefUpdateEvent
    {
        public string Email { get; set; }
        public GcpRefUpdates RefUpdates { get; set; }
    }
}
