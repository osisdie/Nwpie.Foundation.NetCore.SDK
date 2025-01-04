using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.S3Proxy.Contract
{
    public class S3ProxyServiceConfig : IServiceConfig
    {
        public const string ProxyName = "todo-app";
        public const string ServiceName = "s3proxy";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "S3Proxy";
        public const string SysName = "s3proxy";
        public List<string> Tags { get; } = new List<string> { "s3proxy", "storage", "foundation" };
        List<string> IServiceConfig.Tags => Tags;
        public const string BucketConfigKeyForQC = SysConfigKey.PrefixKey_AWS_S3_Credential_ConfigKey + "todo_db";
        public static string S3ServerlessBaseUrl { get; set; }

        public static bool IsProxyRequest(string rawUrl)
        {
            return true == rawUrl?.Contains($"/{ProxyName}", StringComparison.OrdinalIgnoreCase);
        }
    }
}
