namespace Nwpie.Foundation.Abstractions.Statics
{
    public static class SysVarConst
    {
        public const string MachineName = "MACHINE_NAME";
        public const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
        public const string AspNetCoreUrls = "ASPNETCORE_URLS";
        public const string AwsSecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        public const string AwsAccessKeyId = "AWS_ACCESS_KEY_ID";

        public static readonly string SdkApiBearerToken = $"{CommonConst.SdkPrefix.ToUpper()}_API_TOKEN";
        public static readonly string SdkApiName = $"{CommonConst.SdkPrefix.ToUpper()}_API_NAME";
        public static readonly string SdkApiKey = $"{CommonConst.SdkPrefix.ToUpper()}_API_KEY";
        public static readonly string SdkConfigServiceUrl = $"{CommonConst.SdkPrefix.ToUpper()}_CONFIG_SERVICE_URL";
        public static readonly string SdkApiBaseUrl = $"{CommonConst.SdkPrefix.ToUpper()}_API_BASE_URL";
        public static readonly string SdkLocServiceUrl = $"{CommonConst.SdkPrefix.ToUpper()}_LOC_SERVICE_URL";
        public static readonly string SdkAuthServiceUrl = $"{CommonConst.SdkPrefix.ToUpper()}_AUTH_SERVICE_URL";
        public static readonly string SdkNotifyServiceUrl = $"{CommonConst.SdkPrefix.ToUpper()}_NOTIFY_SERVICE_URL";
    }
}
