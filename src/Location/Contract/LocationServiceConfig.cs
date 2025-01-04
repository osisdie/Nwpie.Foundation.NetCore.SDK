using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Location.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class LocationServiceConfig : IServiceConfig
    {
        public const string ServiceName = "loc";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "Location";
        public const string SysName = "loc";
        public const string QueueNameFormat = "{0}-location-{1}";
        public List<string> Tags { get; } = new List<string> { "loc", "location", "foundation" };
        public static readonly string ConfigServiceName = "configserver";

        public static string CreateQueueName(string apiName, string machineName)
        {
            return string.Format(QueueNameFormat, apiName.Replace('.', '-'), machineName)
                .ToLower();
        }
    }
}
