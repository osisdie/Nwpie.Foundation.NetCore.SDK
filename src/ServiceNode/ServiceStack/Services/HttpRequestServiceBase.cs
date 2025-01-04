using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Http.Common.Interfaces;
using Nwpie.Foundation.Http.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Web;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class HttpRequestServiceBase : Service, IRequestService
    {
        public HttpRequestServiceBase()
        {
            Logger = LogMgr.CreateLogger(GetType());
            Serializer = new DefaultSerializer();

            // Allow null
            //TokenService = GetTokenService();

            // Allow null
            if (null != HttpHelper.HttpContext)
            {
                if (null == HttpHelper.HttpContext.Items)
                {
                    HttpHelper.HttpContext.Items = new Dictionary<object, object>();
                }

                HttpHelper.HttpContext.Items.Add(nameof(IRequestService), this);
            }

            Initialization();
        }

        protected virtual void Initialization() { }

        protected virtual async Task LogResult(IRequest req, object responseDto)
        {
            var tokenModel = await GetTokenDetail(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
            var log = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.Response.GetDisplayName() },
                { SysLoggerKey.Contract, req?.OperationName },
                { SysLoggerKey.Requester, tokenModel?.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel?.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel?.GetAccountLevel() },
                { SysLoggerKey.ResponseDto, responseDto },
                { SysLoggerKey.RequestDto, req?.Dto },
                { SysLoggerKey.Headers, req?.Headers },
                { SysLoggerKey.Url, req?.RawUrl },
                { SysLoggerKey.ClientIP, req?.RemoteIp },
                { SysLoggerKey.ConversationId, GetConversationId() },
            }.AddTraceData(_ts);

            Logger.LogInformation(Serializer.Serialize(log));
        }

        protected virtual async Task LogErrorResult(IRequest req, object responseDto, Exception ex)
        {
            var tokenModel = await GetTokenDetail(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
            var log = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, LoggingTypeEnum.ResponseException.GetDisplayName() },
                { SysLoggerKey.Contract, req?.OperationName },
                { SysLoggerKey.Requester, tokenModel?.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel?.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel?.GetAccountLevel() },
                { SysLoggerKey.ResponseDto, responseDto },
                { SysLoggerKey.RequestDto, req?.Dto },
                { SysLoggerKey.Headers, req?.Headers },
                { SysLoggerKey.Url, req?.RawUrl },
                { SysLoggerKey.ClientIP, req?.RemoteIp },
                { SysLoggerKey.ConversationId, GetConversationId() },
                { SysLoggerKey.Exception, ex?.ToString() },
            }.AddTraceData(_ts);

            Logger.LogError(Serializer.Serialize(log));
        }

        public HttpRequest CurrentRequest
        {
            get => HttpHelper.HttpContext?.Request
                ?? (base.Request?.OriginalRequest as HttpRequest);
        }

        public virtual async Task<ITokenDataModel> GetTokenDetail(AuthExactFlagEnum flags)
        {
            if (HttpHelper.HttpContext.TryGet<ITokenDataModel>(nameof(ITokenDataModel), out var saved))
            {
                return saved;
            }

            // Bearer Token first
            if (flags.HasFlag(AuthExactFlagEnum.AuthorizationHeader) ||
                flags.HasFlag(AuthExactFlagEnum.TokenQueryString))
            {
                var token = TokenUtils.GetTokenFromHeaderOrQuery(CurrentRequest);
                if (token.HasValue())
                {
                    // For non-auth applications
                    var profile = await TokenProfileMgr.Instance.ValidateAndQueryTokenAsync<AccountProfileBase>(token, null);
                    if (null != profile)
                    {
                        var lv = (byte)TokenLevelEnum.EndUser;
                        if (profile.Metadata.HasValue())
                        {
                            var dict = Serializer.Deserialize<Dictionary<string, string>>(profile.Metadata, ignoreException: true)
                                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            var metadata = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
                            if (metadata.TryGetValue(nameof(TokenDataModel.LV).ToLower(), out var strLV))
                            {
                                byte.TryParse(strLV, out lv);
                            }
                        }

                        var tokenDto = new TokenDataModel
                        {
                            AccountId = profile.AccountId,
                            Name = profile.Name,
                            Kind = (byte)TokenKindEnum.AccessToken,
                            LV = lv,
                            Exp = DateTime.UtcNow.AddDays(1),
                            Upt = DateTime.UtcNow,
                            Iat = DateTime.UtcNow
                        };

                        HttpHelper.HttpContext.Save(nameof(ITokenDataModel), tokenDto);
                        return tokenDto;
                    }
                }
            }

            // else ApiKey
            if (flags.HasFlag(AuthExactFlagEnum.ApiKeyHeader))
            {
                var apiKey = TokenUtils.GetApiKeyFromHeaderOrQuery(CurrentRequest);
                if (apiKey.HasValue())
                {
                    // For non-auth applications
                    var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
                    if (null != profile)
                    {
                        var tokenDto = new TokenDataModel
                        {
                            AccountId = profile.AccountId,
                            Name = profile.Name,
                            ApiKey = apiKey,
                            Kind = (byte)TokenKindEnum.AccessToken,
                            LV = (byte)TokenLevelEnum.ApplicationUser,
                            Exp = DateTime.UtcNow.AddDays(1),
                            Upt = DateTime.UtcNow,
                            Iat = DateTime.UtcNow
                        };

                        HttpHelper.HttpContext.Save(nameof(ITokenDataModel), tokenDto);
                        return tokenDto;
                    }
                }
            }

            return default(ITokenDataModel);
        }

        public virtual Guid? GetConversationId() => Id;

        public virtual string GetRequester(AuthExactFlagEnum flags)
        {
            if (HttpHelper.HttpContext.TryGet<string>(nameof(GetRequester), out var saved))
            {
                return saved;
            }

            var detail = GetTokenDetail(flags).ConfigureAwait(false).GetAwaiter().GetResult();
            var requster = detail?.GetRequester();
            if (null != requster)
            {
                HttpHelper.HttpContext.Save(nameof(GetRequester), requster);
                return requster;
            }

            return null;
        }

        public virtual async Task<T> GetProfile<T>(AuthExactFlagEnum flags)
            where T : AccountProfileBase
        {
            if (HttpHelper.HttpContext.TryGet<T>(nameof(GetProfile), out var saved))
            {
                return saved;
            }

            var accountId = GetAccountId(flags);
            if (accountId.HasValue())
            {
                var profile = await TokenProfileMgr.Instance.GetProfileByAccountIdAsync<T>(accountId);
                if (null != profile)
                {
                    HttpHelper.HttpContext.Save(nameof(GetProfile), profile);
                    return profile;
                }
            }

            //var apiKey = await TokenUtils
            //    .ExtractApiKey(CurrentRequest,
            //        AuthExactFlagEnum.ApiKeyHeader |
            //        AuthExactFlagEnum.ApiKeyInToken
            //    );
            //if (apiKey.HasValue())
            //    return await TokenProfileMgr.Instance.GetApiKeyProfileAsync<T>(apiKey);

            return default(T);
        }

        public virtual string GetAccountId(AuthExactFlagEnum flags)
        {
            if (HttpHelper.HttpContext.TryGet<string>(nameof(GetAccountId), out var saved))
            {
                return saved;
            }

            var detail = GetTokenDetail(flags).ConfigureAwait(false).GetAwaiter().GetResult();
            var accountId = detail?.GetAccountId();
            if (null != accountId)
            {
                HttpHelper.HttpContext.Save(nameof(GetAccountId), accountId);
                return detail.GetAccountId();
            }

            return null;
        }

        public virtual string GetRequestRemoteIP() =>
            string.IsNullOrWhiteSpace(Request?.RemoteIp)
                ? string.Empty
                : Request?.RemoteIp
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.First();

        public virtual string GetRequestUserAgent() =>
            string.IsNullOrWhiteSpace(Request?.UserAgent)
                ? string.Empty
                : Request?.UserAgent;

        public virtual ITokenService GetTokenService() =>
            ComponentMgr.Instance.GetDefaultJwtTokenService()
                ?? ComponentMgr.Instance.GetDefaultTokenService();

        public ILogger Logger { get; private set; }
        public ISerializer Serializer { get; private set; }
        //public ITokenService TokenService { get; private set; }
        public DateTime _ts { get; private set; } = DateTime.UtcNow;
        public Guid? Id { get; private set; } = Guid.NewGuid();
    }
}
