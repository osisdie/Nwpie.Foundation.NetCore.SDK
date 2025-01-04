namespace Nwpie.Foundation.Abstractions.Logging
{
    public static class SysLoggerKey
    {
        // Common
        public const string ElapsedSeconds = "elapsedSeconds";
        public const string ApiKey = "apikey";
        public const string ApiName = "apiname";
        public const string Service = "svc";
        public const string Version = "ver";
        public const string Environment = "env";
        public const string IP = "_ip";
        public const string HostName = "_host";
        public const string Platform = "_os";
        public const string TimeStamp = "_ts";
        public const string UpTime = "_up";

        // Foundation
        public const string Provider = "provider";
        public const string CacheKey = "cacheKey";
        public const string Caller = "caller";
        public const string SrcFilePath = "srcFilePath";
        public const string IsTimeout = "isTimeout";
        public const string Database = "database";
        public const string ConnectionGuid = "connectionGuid";
        public const string ConnectionTimeout = "connectionTimeout";
        public const string CommandTimeout = "commandTimeout";
        public const string CommandText = "commandText";
        public const string CommandType = "commandType";
        public const string CommandGuid = "commandGuid";
        public const string Paramters = "paramters";
        public const string FilePattern = "filePattern";
        public const string Files = "files";
        public const string Path = "path";
        public const string Insight = "inspection";

        // HTTP
        public const string Type = "type";
        public const string Headers = "header";
        public const string Contract = "contract";
        public const string Requester = "requester";
        public const string AccountId = "accountId";
        public const string AccountLevel = "accountLevel";
        public const string ResponseDto = "response";
        public const string RequestDto = "request";
        public const string Url = "url";
        public const string ClientIP = "clientIP";
        public const string Exception = "exception";
        public const string ConversationId = "conversationId";

        // Timestamp
        public const string MillisecondsDuration = "ms";
    }
}
