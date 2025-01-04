using System.Collections.Generic;

namespace Nwpie.Foundation.Common.Notification.Models
{
    public class LineEventsRequest
    {
        public List<LineEventModel> Events { get; set; }
    }

    public class LineEventModel
    {
        public string ReplyToken { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public LineSource Source { get; set; }
        public LineMessage Message { get; set; }
    }

    public class LineSource
    {
        public string Type { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string RoomId { get; set; }
    }

    public class LineMessage
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
