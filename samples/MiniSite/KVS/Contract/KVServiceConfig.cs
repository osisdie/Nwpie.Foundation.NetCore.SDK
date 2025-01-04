using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.MiniSite.KVS.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class KVServiceConfig : IServiceConfig
    {
        public const string ServiceName = "kvs";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "Config";
        public const string SysName = "kvs";
        public List<string> Tags { get; } = new List<string> { "kvs", "config", "foundation" };
    }
}
