using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.MiniSite.ES.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class ESProxyServiceConfig : IServiceConfig
    {
        public const string ServiceName = "es";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "ES";
        public const string SysName = "es";
        public static List<string> Tags { get; } = new List<string> { "es", "elasticsearch", "esproxy", "foundation" };
        List<string> IServiceConfig.Tags => Tags;
        public static List<string> Indices { get; set; } = new List<string>();
        public static string ElasticSearchBaseUrl { get; set; }
    }
}
