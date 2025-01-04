using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Abstractions.ServiceNode
{
    public static class SNConst
    {
        public static readonly string HTTPHeader_RequestID = $"{CommonConst.SdkPrefix}-ReqCtxRequestID";
        public static readonly string HTTPHeader_Depth = $"{CommonConst.SdkPrefix}-ReqCtxDepth";
        public static readonly string HTTPHeader_SequenceNum = $"{CommonConst.SdkPrefix}-ReqCtxSequenceNum";
        public static readonly string HTTPHeader_CallingService = $"{CommonConst.SdkPrefix}-ReqCtxCallingService";
        public static readonly string HTTPHeader_ExecutingService = $"{CommonConst.SdkPrefix}-ReqCtxExecutingService";

        public static readonly string HTTPHeader_ConsumeIP = $"X-{CommonConst.SdkPrefix}-ConsumeIP";
        public static readonly string HTTPHeader_AppKey = $"X-{CommonConst.SdkPrefix}-AppKey";
        public static readonly string HTTPHeader_Token = $"X-{CommonConst.SdkPrefix}-Token";
        public static readonly string HTTPHeader_SecurityKey = $"X-{CommonConst.SdkPrefix}-SecurityKey";

        public const string HealthCheckController = "HealthCheck";
    }
}
