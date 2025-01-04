using System.IO;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class FileUtils
    {
        public static string GetEnvironmentFileName(string envName, string filenameWithExt, bool checkExists = false, string checkExtraFolder = null)
        {
            if (string.IsNullOrWhiteSpace(envName))
            {
                return filenameWithExt;
            }

            var sysEnv = Enum<EnvironmentEnum>.TryParse(envName, EnvironmentEnum.Max);
            if ((int)sysEnv <= (int)EnvironmentEnum.Debug)
            {
                if (File.Exists(filenameWithExt.CombineAppFolder()))
                {
                    return filenameWithExt;
                }

                if (checkExtraFolder.HasValue() &&
                    File.Exists(Path.Combine(checkExtraFolder, filenameWithExt).CombineAppFolder()))
                {
                    return Path.Combine(checkExtraFolder, filenameWithExt);
                }

                return filenameWithExt;
            }

            var envFileName = $"{Path.GetFileNameWithoutExtension(filenameWithExt)}.{envName}{Path.GetExtension(filenameWithExt)}";
            if (checkExists)
            {
                if (File.Exists(envFileName.CombineAppFolder()))
                {
                    return envFileName;
                }

                if (checkExtraFolder.HasValue() &&
                    File.Exists(Path.Combine(checkExtraFolder, envFileName).CombineAppFolder()))
                {
                    return Path.Combine(checkExtraFolder, envFileName);
                }

                return filenameWithExt;
            }

            return envFileName;
        }
    }
}
