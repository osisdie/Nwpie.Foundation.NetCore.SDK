using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Common.Location
{
    public static class LocatonUtils
    {
        public static string SDK_ApiEnvironmentFilePath_DependsOnEnv(string relativeFolder = ConfigConst.DefaultAppDataFolder) =>
          FileUtils.GetEnvironmentFileName(
             envName: ServiceContext.ASPNETCORE_ENVIRONMENT,
             filenameWithExt: ConfigConst.DefaultApiEnvironmentConfigFile,
             checkExists: true,
             checkExtraFolder: relativeFolder
                ?? ConfigConst.DefaultAppDataFolder
         );
    }
}
