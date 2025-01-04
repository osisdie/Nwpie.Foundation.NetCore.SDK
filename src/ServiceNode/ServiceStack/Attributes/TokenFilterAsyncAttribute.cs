using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Measurement;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Web;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Attributes
{
    public abstract class TokenFilterAsyncAttribute : RequestFilterAsyncAttribute
    {
        static TokenFilterAsyncAttribute()
        {
            Logger = LogMgr.CreateLogger(typeof(TokenFilterAsyncAttribute));
            CacheClient = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            Serializer = new DefaultSerializer();
        }

        public TokenFilterAsyncAttribute()
        {

        }

        public TokenFilterAsyncAttribute(TokenLevelEnum minLevel)
        {
            Level = minLevel;
        }

        public TokenFilterAsyncAttribute(string role, params string[] roles)
        {
            var list = new List<string>();
            if (role.HasValue())
            {
                list.Add(role);
            }

            if (roles?.Count() > 0)
            {
                list.AddRange(roles);
            }

            m_RequireRoles = list.Distinct().ToList();
        }

        public override async Task ExecuteAsync(IRequest req, IResponse resp, object requestDto)
        {
            var isDone = false;

            // 1st apiKey
            isDone = await VerifyApiKeyIfExists(req, resp);
            if (isDone)
            {
                return;
            }

            // 2nd bearer token
            isDone = await VerifyBearerTokenIfExists(req, resp);
            if (isDone)
            {
                return;
            }

            AuthSDKMeasurementExtension.WriteBadToken(nameof(string.Empty), GetType().Name, nameof(string.Empty));
            await ResponseForbidden(req, resp, StatusCodeEnum.MissingAccessToken);
            return;
        }

        public virtual async Task<bool> VerifyApiKeyIfExists(IRequest req, IResponse resp)
        {
            var currentRequest = req.OriginalRequest as HttpRequest;
            var isDone = false;

            var apiKey = currentRequest.GetValueFromHeaderOrQuery(CommonConst.ApiKey);
            if (apiKey.HasValue())
            {
                isDone = true;
                m_AccountProfile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
                if (null == m_AccountProfile)
                {
                    AuthSDKMeasurementExtension.WriteBadToken(apiKey, typeof(DefaultApiKeyAuthService<>).Name, apiKey);

                    await ResponseForbidden(req, resp, StatusCodeEnum.InvalidApiKeyCredential);

                    return isDone;
                }

                OnVerified(req, resp, m_AccountProfile);
                return isDone; // pass
            }

            return isDone;
        }

        public virtual async Task<bool> VerifyBearerTokenIfExists(IRequest req, IResponse resp)
        {
            var currentRequest = req.OriginalRequest as HttpRequest;
            var isDone = false;

            var bearerToken = TokenUtils.GetTokenFromHeaderOrQuery(currentRequest);
            if (bearerToken.HasValue())
            {
                isDone = true;
                try
                {
                    m_AccountProfile = await TokenProfileMgr.Instance.ValidateAndQueryTokenAsync<AccountProfileBase>(bearerToken,
                        new TokenDataModel()
                        {
                            IP = req.RemoteIp,
                            UA = req.UserAgent
                        }
                    );

                    if (null == m_AccountProfile)
                    {
                        throw new Exception(StatusCodeEnum.InvalidAccessToken.GetDisplayName());
                    }

                    AuthSDKMeasurementExtension.WriteOKToken(bearerToken.ToMD5(), GetType().Name, m_AccountProfile.Name);

                    OnVerified(req, resp, m_AccountProfile);

                    return isDone; // pass
                }
                catch (Exception ex)
                {
                    AuthSDKMeasurementExtension.WriteBadToken(bearerToken.ToMD5(), GetType().Name, ex.Message);

                    var e = true == ex.Message?.Contains("expire", StringComparison.OrdinalIgnoreCase)
                        ? StatusCodeEnum.AccessTokenExpired
                        : StatusCodeEnum.InvalidAccessToken;
                    await ResponseForbidden(req, resp, e);

                    return isDone;
                }
            }

            return isDone;
        }

        public virtual async Task<bool?> IsValidAccount(ITokenDataModel tokenModel)
        {
            await Task.CompletedTask;
            return true == tokenModel?.GetAccountId().HasValue();
        }

        public virtual async Task<bool?> IsValidAccount(IAccountProfile profile)
        {
            await Task.CompletedTask;
            return true == profile?.AccountId.HasValue();
        }

        public virtual async Task<bool?> IsValidLevel(ITokenDataModel tokenModel)
        {
            if (false == tokenModel?.LV.HasValue ||
                Level <= TokenLevelEnum.UnSet ||
                Level >= TokenLevelEnum.Max)
            {
                return true;
            }

            await Task.CompletedTask;
            return tokenModel?.LV >= (byte)Level;
        }

        public virtual async Task<bool?> IsValidRole(List<string> roles)
        {
            await Task.CompletedTask;
            return true;
        }

        public virtual void OnError(IRequest request, IResponse response, StatusCodeEnum code) { }
        public virtual void OnVerified(IRequest request, IResponse response, IAccountProfile profile) { }

        /// <summary>
        /// Normal case: Bear + JwtToken
        /// QC special case: AesToken  (NO Bearer)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual ITokenService GetTokenService(HttpRequest request) =>
            ComponentMgr.Instance.GetDefaultJwtTokenService();

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
                { SysLoggerKey.Requester, tokenModel.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel.GetAccountLevel() },
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
        public TokenLevelEnum Level { get; set; } = TokenLevelEnum.EndUser; // Mininum level

        public static readonly ILogger Logger;
        public static readonly ISerializer Serializer;
        public static ICache CacheClient;

        protected readonly List<string> m_RequireRoles;
        protected ITokenDataModel m_TokenModel;
        protected IAccountProfile m_AccountProfile;
    }
}
