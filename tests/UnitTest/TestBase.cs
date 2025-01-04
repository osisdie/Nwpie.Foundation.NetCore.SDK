using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.xUnit.Extension;
using Nwpie.xUnit.App_Start;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit
{
    public abstract class TestBase : CObject
    {
        static TestBase()
        {
            if (false == m_IsInitialized)
            {
                lock (m_Lock)
                {
                    if (false == m_IsInitialized)
                    {
                        WarnUp();
                        m_IsInitialized = true;
                    }
                }
            }
        }

        public TestBase(ITestOutputHelper output)
        {
            m_Output = output;
            Serializer = ComponentMgr.Instance.SerializerFromDI;
            DefaultLocalCache = ComponentMgr.Instance.LocalCacheFromDI;
            DefaultConfigServer = ComponentMgr.Instance.TryResolve<IConfigClient>();

            try
            {
                IsReady().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual async Task<bool> IsReady()
        {
            if (!Directory.Exists($"/{ConfigConst.DefaultTempFolder}"))
            {
                Directory.CreateDirectory($"/{ConfigConst.DefaultTempFolder}");
            }

            await Task.CompletedTask;
            return true;
        }

        private static void WarnUp()
        {
            if (m_IsInitialized)
            {
                return;
            }

#if DEBUG
            LaunchSettingsExtension.SetEnvironmentVariables();
#endif
            ServiceContext.Initialize();
            Assert.NotNull(ServiceContext.ConfigServiceUrl);
            Assert.NotNull(ServiceContext.ApiKey);
            Assert.NotNull(ServiceContext.ApiName);

            DefaultLogUtil.InitialLog4netProvider(overwriteFactory: true);
            DefaultConfigUtil.InitialConfigure();
            DefaultComponentUtil.InitialAutofac();

            Assert.True(ComponentMgr.Instance.IsReady);
        }

        public string GetMapValueDefault(string key, string env, EnvironmentEnum defaultEnv = EnvironmentEnum.Debug)
        {
            if (ConfigKeyForSetForAllEnvMap[key].Any(o => o.Key == env))
            {
                return ConfigKeyForSetForAllEnvMap[key]
                    .Where(o => o.Key == env)
                    ?.FirstOrDefault()
                    .Value;
            }

            return ConfigKeyForSetForAllEnvMap[key]
                .Where(o => o.Key == defaultEnv.GetDisplayName())
                ?.FirstOrDefault()
                .Value;
        }

        public const string __ = CommonConst.Separator;
        public const string DefaultTestEmail = "dev@kevinw.net";
        public const string DefaultPassword = "**";
        public const string DefaultTestAccountId = "fake_user_id";

        public static Dictionary<string, string> ConfigServiceUrlMap = new(StringComparer.OrdinalIgnoreCase);
        public static Dictionary<string, string> LocationServiceUrlMap = new(StringComparer.OrdinalIgnoreCase);
        public static Dictionary<string, string> AuthServiceUrlMap = new(StringComparer.OrdinalIgnoreCase);
        public static List<KeyValuePair<string, string>> AdminApiKeyList = new();
        public static List<KeyValuePair<string, string>> NtfyApiKeyList = new();
        public static List<KeyValuePair<string, string>> TodoApiKeyList = new();
        public static List<ConfigItem> ConfigKeyForReadList = new();
        public static Dictionary<string, Dictionary<string, string>> ConfigKeyForSetForAllEnvMap = new(StringComparer.OrdinalIgnoreCase);
        public static IConfigClient DefaultConfigServer;
        public static ISerializer Serializer;
        public static ILocalCache DefaultLocalCache;

        protected readonly ITestOutputHelper m_Output;

        private static readonly bool m_IsInitialized = false;
        private static readonly object m_Lock = new();
    }
}
