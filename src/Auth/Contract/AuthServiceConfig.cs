using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Auth.Contract
{
    /// <summary>
    /// Const class provide config definition of service node.
    /// </summary>
    public class AuthServiceConfig : IServiceConfig
    {
        public const string DefaultDatabaseName = "sys_db";
        public const string ServiceName = "auth";
        string IServiceConfig.ServiceName => ServiceName;
        public const string ControllerName = "Auth";
        public const string SysName = "auth";
        public List<string> Tags { get; } = new List<string> { "auth", "foundation" };

        public const int MinHierarchy = 2;
        public const int MaxHierarchy = 5;
        public const string AppNamePattern = @"^[a-z0-9_.]+$";
        public const string ApiNamePattern = @"^[a-z0-9_.]+.((base)|(debug)|(dev)|(stage)|(preprod)|(prod)){1}$";
        public static readonly Regex MatchApiNamePattern = new Regex(ApiNamePattern);

        public static Regex MatchEnvApiNamePattern(string currentEnv = null)
        {
            currentEnv = currentEnv ?? SdkRuntime.SdkEnv;
            var sdkEnv = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(currentEnv, EnvironmentEnum.Max);
            if (EnvironmentEnum.Production == sdkEnv || EnvironmentEnum.Staging_2 == sdkEnv)
            {
                return new Regex(@"^[a-z0-9_.]+.((base)|(preprod)|(prod)){1}$");
            }

            return new Regex(@"^[a-z0-9_.]+.((base)|(debug)|(dev)|(stage)){1}$");
        }
    }
}
