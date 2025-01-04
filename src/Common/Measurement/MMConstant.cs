using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Common.Measurement
{
    public static class MMConstant
    {
        public static readonly string HTTPHeader_RequestID = $"{CommonConst.SdkPrefix}ReqCtxRequestID";
        public static readonly string HTTPHeader_Depth = $"{CommonConst.SdkPrefix}ReqCtxDepth";
        public static readonly string HTTPHeader_SequenceNum = $"{CommonConst.SdkPrefix}ReqCtxSequenceNum";
        public static readonly string HTTPHeader_CallingService = $"{CommonConst.SdkPrefix}ReqCtxCallingService";
        public static readonly string HTTPHeader_ExecutingService = $"{CommonConst.SdkPrefix}ReqCtxExecutingService";

        public static readonly string HTTPHeader_ConsumeIP = $"{CommonConst.SdkPrefix}ConsumeIP";
        public const string HTTPHeader_ApiKey = "ApiKey";

        public const string DefaultServiceName = "Unknown Service";

        public const string RequestTimeLine_MeasurementName = "test_write4_timeline";

        public const string RequestTimeLine_RequestID = "request_id";
        public const string RequestTimeLine_Depth = "depth";
        public const string RequestTimeLine_ServerName = "server_name";
        public const string RequestTimeLine_ServerIP = "server_ip";
        public const string RequestTimeLine_ProcessID = "process_id";
        public const string RequestTimeLine_ThreadID = "thread_id";
        public const string RequestTimeLine_ServiceName = "service_name";
        public const string RequestTimeLine_MethodName = "method_name";
        public const string RequestTimeLine_MethodIn = "method_in";
        public const string RequestTimeLine_MethodExecTime = "method_exec_time";
        public const string RequestTimeLine_Time = "time";
        public const string RequestTimeLine_SequenceNum = "sequence_num";

        public const string MeasurementScope_Statistic_Hit = "hit";
        public const string MeasurementScope_Statistic_MethodExecTime = "method_exec_time";
        public const string MeasurementScope_Placeholder_HTTP_Call = "MeasurementScope_Placeholder_HTTP_Call";

        public const string MetircNameRequestTrace = "_mscope_request_trace";
        public const string MetircNameScopeStatistic = "_mscope_statistic";

        public const string CommandPara_DBName = "dbname";
        public const string CommandPara_QueryText = "query";

        public const string DateTimeFormatOfMetircPoint = "yyyy/MM/dd HH:mm:ss.ffffff";

        // for measurement service
        public const string MeasurementEnabled = "foundation:apm:measurementEnabled";
        public const string MeasurementKeyDBUser = "foundation:apm:mmdbuser";
        public const string MeasurementKeyDBPwd = "foundation:apm:mmdbpwd";
        public const string MeasurementKeyGlobalDefaultDB = "foundation:apm:mmdefaultdb";
        public const string MeasurementKeyUri = "foundation:apm:mmuri";
        public const string MeasurementMaxMessageCount = "foundation:apm:maxMessageCount";
        public const string MeasurementSendFrequency = "foundation:apm:sendFrequency";
        public const string WorkerQueuePerfCounterEnabled = "foundation:mq:workerQueuePerfCounterEnabled";
    }
}
