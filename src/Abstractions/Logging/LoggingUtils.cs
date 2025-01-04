using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Abstractions.Logging
{
    public static class LoggingUtils
    {
        public static string EnvironmentLog4netFile => SDK_Log4netFilePath_DependsOnEnv();
        public static string EnvironmentNLogFile => SDK_NLogFilePath_DependsOnEnv();

        public static string SDK_Log4netFilePath_DependsOnEnv(string relativeFolder = ConfigConst.DefaultConfigFolder) =>
            FileUtils.GetEnvironmentFileName(
                envName: SdkRuntime.ASPNETCORE_ENVIRONMENT,
                filenameWithExt: ConfigConst.DefaultLog4netConfigFile,
                checkExists: true,
                checkExtraFolder: relativeFolder
                    ?? ConfigConst.DefaultConfigFolder
            );

        public static string SDK_NLogFilePath_DependsOnEnv(string relativeFolder = ConfigConst.DefaultConfigFolder) =>
            FileUtils.GetEnvironmentFileName(
                envName: SdkRuntime.ASPNETCORE_ENVIRONMENT,
                filenameWithExt: ConfigConst.DefaultNLogConfigFile,
                checkExists: true,
                checkExtraFolder: relativeFolder
                    ?? ConfigConst.DefaultConfigFolder
            );
    }
}
