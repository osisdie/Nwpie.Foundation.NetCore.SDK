using System;

namespace Nwpie.Foundation.Auth.Contract.AuditLog.LogEvent
{
    public class AdtLogEvent_RequestModel : AuthParamModel
    {
        public string Kind { get; set; }
        public string TargetId { get; set; }
        public string TargetName { get; set; }
        public string TargetType { get; set; }
        public string Program { get; set; }
        public string Message { get; set; }
        public string JsonMessage { get; set; }
        public string IP { get; set; }
        public string Status { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
    }
}
