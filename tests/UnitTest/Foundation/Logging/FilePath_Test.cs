using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Logging
{
    public class FilePath_Test : TestBase
    {
        public FilePath_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Log4netFilePath_Test()
        {
            // Mapping from debug -> Debug
            // Mapping from dev -> Development
            // Mapping from stage -> Staging
            // Mapping from preprod -> Staging_2
            // Mapping from prod -> Production
            foreach (var env in AdminApiKeyList)
            {
                var configuration = env.Key.ToString().ToLower();
                var e = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(configuration, EnvironmentEnum.Max);
                Assert.NotEqual(EnvironmentEnum.Max, e);
            }

            var checkExists = true;
            var filename = ConfigConst.DefaultLog4netConfigFile;
            Assert.Equal("log4net.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Testing.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
            Assert.Equal("log4net.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Debug.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
            Assert.Equal(@"conf\log4net.Development.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Development.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
            Assert.Equal(@"conf\log4net.Staging.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Staging.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
            Assert.Equal(@"conf\log4net.Staging_2.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Staging_2.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
            Assert.Equal(@"conf\log4net.Production.config", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Production.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultConfigFolder));
        }

        [Fact]
        public void EnvironmentFilePath_Test()
        {
            // Mapping from debug -> Debug
            // Mapping from dev -> Development
            // Mapping from stage -> Staging
            // Mapping from preprod -> Staging_2
            // Mapping from prod -> Production
            foreach (var env in AdminApiKeyList)
            {
                var configuration = env.Key.ToString().ToLower();
                var e = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(configuration, EnvironmentEnum.Max);
                Assert.NotEqual(EnvironmentEnum.Max, e);
            }

            var checkExists = true;
            var filename = ConfigConst.DefaultApiEnvironmentConfigFile;
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Testing.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Debug.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.Development.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Development.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.Staging.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Staging.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.Staging_2.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Staging_2.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
            Assert.Equal($@"{ConfigConst.DefaultAppDataFolder}\Environments.Production.xml", FileUtils.GetEnvironmentFileName(EnvironmentEnum.Production.ToString(), filename, checkExists, checkExtraFolder: ConfigConst.DefaultAppDataFolder));
        }

        public override Task<bool> IsReady()
        {
            Assert.NotEmpty(AdminApiKeyList);

            return base.IsReady();
        }
    }
}
