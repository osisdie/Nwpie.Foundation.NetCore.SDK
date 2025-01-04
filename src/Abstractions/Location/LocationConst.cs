namespace Nwpie.Foundation.Abstractions.Location
{
    public static class LocationConst
    {
        public const int DefaultConfigCacheSecs = 86400 * 7; // 5 days
        public const string LocationEventName = "LocationEvent";
        public const string HttpPathToHealthCheckRequest_Get = "/HealthCheck/HlckHelloRequest";
        public const string HttpPathToConfigContractControllerName = "configserver";
        public const string HttpPathToConfigContractRequest_Get = "/get";
        public const string HttpPathToConfigContractRequest_Set = "/set";
        public const string HttpPathToConfigContractRequest_Refresh = "/refresh";
        public const string HttpPathToLocationContractRequest_Refresh = "/location/refresh";
        public const string HttpPathToS3ProxyContractRequest_Upload = "/s3proxy/upload";
        public const string HttpPathToS3ProxyContractRequest_Download = "/s3proxy/download";
    }
}
