using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Storage.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class StorageServiceConfig : IServiceConfig
    {
        public const string ServiceName = "storage";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "Storage";
        public const string SysName = "fs";
        public List<string> Tags { get; } = new List<string> { "storage", "file", "foundation" };
    }
}
