using System;
using System.Collections.Concurrent;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Common
{
    public class ConfigMgr_Test : TestBase
    {
        public ConfigMgr_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Config_Upsert_Test()
        {
            var config = new ConcurrentDictionary<Type, IOption>();
            var config1 = new Platform_Option() { SdkEnv = "debug" };
            var config2 = new Platform_Option() { SdkEnv = "dev" };
            var config3 = new Platform_Option() { SdkEnv = "stage" };

            //config.TryRemove(typeof(Platform_Cfg), out var removedConfig);
            //config.TryAdd(typeof(Platform_Cfg), config1);
            //config.TryAdd(typeof(Platform_Cfg), config2);
            config.AddOrUpdate(typeof(Platform_Option),
                config1,
                (k, v) => config1
            );
            Assert.Equal(config1.SdkEnv,
                (config[typeof(Platform_Option)] as Platform_Option)?.SdkEnv);

            config.AddOrUpdate(typeof(Platform_Option),
                config2,
                (k, v) => config2
            );
            Assert.Equal(config2.SdkEnv,
                (config[typeof(Platform_Option)] as Platform_Option)?.SdkEnv);

            config[typeof(Platform_Option)] = config3;
            Assert.Equal(config3.SdkEnv,
                (config[typeof(Platform_Option)] as Platform_Option)?.SdkEnv);
        }
    }
}
