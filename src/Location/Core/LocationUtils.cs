using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Location.Contract;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;

namespace Nwpie.Foundation.Location.Core
{
    public static class LocationUtils
    {
        public static string GetFoundationLocation(string apiName, string myApiKey, string ip = null) =>
            LocationHost.Instance
                .GetApiLocation(new LocGetApiLocation_Request
                {
                    ApiKey = myApiKey,
                    AppName = "foundation",
                    EnvInfo = new EnvInfo
                    {
                        Env = Utility.GetSDKEnvNameByApiName(apiName),
                        IP = ip
                    }
                })?.Data;

        public static string AttachPrefix(this string configKey, string apiName) =>
            $"{ConfigCacheKeyPrefix}{__}{Utility.GetSDKEnvNameByApiName(apiName)}{__}{configKey}";

        public static string DetachPrefix(this string cacheKey, string apiName) =>
            cacheKey?.Replace($"{ConfigCacheKeyPrefix}{__}{Utility.GetSDKEnvNameByApiName(apiName)}{__}", string.Empty);

        public static string AttachApiNameSuffix(this string cacheKey, string apiName) =>
            $"{cacheKey}{__}{apiName}";

        public static string DetachApiNameSuffix(this string cacheKey, string apiName) =>
            cacheKey?.Replace($"{__}{apiName}", string.Empty);

        public const string ConfigCacheDivider = "--";
        public const string __ = ConfigCacheDivider;
        public static readonly string ConfigCacheKeyPrefix = $"{LocationServiceConfig.ServiceName}{__}config";
    }
}
