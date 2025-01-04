using System;

namespace Nwpie.Foundation.Abstractions.Statics
{
    public static class CommonConst
    {
        public const string SdkPrefix = "Sdk";
        public const string ServiceName = "Svc";
        public const string ApiName = "ApiName";
        public const string ApiKey = "ApiKey";
        public const string AppName = "AppName";
        public const string PAT = "PAT";
        public const string ServiceEnv = "Env";
        public const string ServiceLocalIP = "LocalIP";
        public const string FriendAppName = "FriendAppName";
        public const string ValidateMsgKey = "ValidateMsg";
        public const string AuthHeaderName = "Authorization";
        public const string TokenPropertyName = "token";
        public const string AccessTokenHeaderName = "accessToken";
        public const string RefreshTokenHeaderName = "refreshToken";
        public const string Separator = "__";
        public const string CharsetUTF8 = "utf-8";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string DateTimeFormat_MS = "yyyy-MM-dd HH:mm:ss.fff";

        public static readonly DateTime UnixBaseTime = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;
    }
}
