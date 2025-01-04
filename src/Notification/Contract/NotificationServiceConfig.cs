using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Notification.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class NotificationServiceConfig : IServiceConfig
    {
        public const string ServiceName = "hub";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "Notification";
        public const string SysName = "ntfy";
        public List<string> Tags { get; } = new List<string> { "hub", "ntfy", "notification", "foundation" };
    }
}
