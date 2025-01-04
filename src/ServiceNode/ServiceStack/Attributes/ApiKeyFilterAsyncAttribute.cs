using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Measurement;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.ServiceNode.ServiceStack.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Web;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Attributes
{
    public abstract class ApiKeyFilterAsyncAttribute : RequestFilterAsyncAttribute
    {
        static ApiKeyFilterAsyncAttribute()
        {
            Logger = LogMgr.CreateLogger(typeof(ApiKeyFilterAsyncAttribute));
            CacheClient = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            Serializer = new DefaultSerializer();
        }

        public override async Task ExecuteAsync(IRequest req, IResponse res, object requestDto)
        {
            var currentRequest = req.OriginalRequest as HttpRequest;
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                ApiKey = await TokenUtils.ExtractApiKey(currentRequest,
                    AuthExactFlagEnum.ApiKeyHeader |
                    AuthExactFlagEnum.ApiKeyInToken
                );

                if (string.IsNullOrWhiteSpace(ApiKey))
                {
                    await ResponseForbidden(req, res, StatusCodeEnum.MissingApiKey);
                    return;
                }
            }

            var isValid = await IsValidApiKey(ApiKey);
            if (true != isValid)
            {
                AuthSDKMeasurementExtension.WriteBadToken(ApiKey, typeof(DefaultApiKeyAuthService<>).Name, ApiKey);
                await ResponseForbidden(req, res, StatusCodeEnum.InvalidApiKeyCredential);
                return;
            }

            AuthSDKMeasurementExtension.WriteOKToken(ApiKey, typeof(DefaultApiKeyAuthService<>).Name, ApiKey);
        }

        public virtual async Task<bool?> IsValidApiKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            m_AccountProfile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
            return null != m_AccountProfile;
        }

        public virtual void OnError(IRequest req, IResponse res, StatusCodeEnum code) { }
        public virtual async Task ResponseForbidden(IRequest req, IResponse res, StatusCodeEnum code, HttpStatusCode httpStatus = HttpStatusCode.Unauthorized)
        {
            OnError(req, res, code);
            var responseDto = new ServiceResponse<object>()
            {
                IsSuccess = false,
                Code = (int)code,
                ErrMsg = code.ToString()
            };

            var tokenModel = await TokenUtils.GetTokenDetail(req?.OriginalRequest as HttpRequest);
            Logger.LogWarning(Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.ResponseException.GetDisplayName() },
                { SysLoggerKey.Contract, req?.OperationName },
                { SysLoggerKey.Requester, tokenModel?.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel?.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel?.GetAccountLevel() },
                { SysLoggerKey.ResponseDto, res?.Dto },
                { SysLoggerKey.RequestDto, req?.Dto },
                { SysLoggerKey.Headers, req?.Headers },
                { SysLoggerKey.Url, req?.RawUrl },
                { SysLoggerKey.ClientIP, req?.RemoteIp },
                { SysLoggerKey.Exception, code.ToString() },
            }.AddTraceData()));

            res.StatusCode = (int)httpStatus;
            res.ContentType = "application/json";
            req.Response.AddCorsHeader();

            await res.WriteAsync(responseDto.ToJson())
                .ContinueWith(t => res.EndRequest(skipHeaders: false));
        }

        public string Description { get; set; }
        public bool AllowExpired { get; set; }
        public string ApiKey { get; set; }
        public string AppName { get; set; }

        public static readonly ILogger Logger;
        public static ISerializer Serializer;
        public static ICache CacheClient;

        protected IAccountProfile m_AccountProfile;
    }
}
