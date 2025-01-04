using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ServiceStack.Web;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Utilities
{
    public static class ServiceNodeUtils
    {
        static ServiceNodeUtils()
        {
            m_Serializer = new DefaultSerializer();
            m_CacheClient = new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                TrackStatistics = true
            }));
        }

        public static void AddCorsHeader(this IResponse resp)
        {
            resp.AddHeader("Access-Control-Allow-Origin", "*");
            resp.AddHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
            resp.AddHeader("Access-Control-Allow-Headers", "Content-Type");
        }

        public static async Task<string> ConvertToRequestLog(IRequest request, object responseDto = null, Exception ex = null)
        {
            var tokenModel = await GetTokenDetail(request);
            return m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, null == ex ? LoggingTypeEnum.Request.GetDisplayName() : LoggingTypeEnum.RequestException.GetDisplayName() },
                { SysLoggerKey.Contract, request?.OperationName },
                { SysLoggerKey.Requester, tokenModel?.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel?.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel?.GetAccountLevel() },
                { SysLoggerKey.ResponseDto, responseDto },
                { SysLoggerKey.RequestDto, request?.Dto },
                { SysLoggerKey.Headers, request?.Headers },
                { SysLoggerKey.Url, request?.RawUrl },
                { SysLoggerKey.ClientIP, request?.RemoteIp },
                { SysLoggerKey.Exception, ex?.ToString() },
            }.AddTraceData());
        }

        public static async Task<string> ConvertToResponseLog(IRequest request, object responseDto, Exception ex = null)
        {
            var tokenModel = await GetTokenDetail(request);
            return m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { SysLoggerKey.Type, null == ex ? LoggingTypeEnum.Response.GetDisplayName() : LoggingTypeEnum.ResponseException.GetDisplayName() },
                { SysLoggerKey.Contract, request?.OperationName },
                { SysLoggerKey.Requester, tokenModel?.GetRequester() },
                { SysLoggerKey.AccountId, tokenModel?.GetAccountId() },
                { SysLoggerKey.AccountLevel, tokenModel?.GetAccountLevel() },
                { SysLoggerKey.ResponseDto, responseDto },
                { SysLoggerKey.RequestDto, request?.Dto },
                { SysLoggerKey.Headers, request?.Headers },
                { SysLoggerKey.Url, request?.RawUrl },
                { SysLoggerKey.ClientIP, request?.RemoteIp },
                { SysLoggerKey.Exception, ex?.ToString() },
            }.AddTraceData());
        }

        public static async Task<ITokenDataModel> GetTokenDetail(IRequest request)
        {
            if (!(request?.OriginalRequest is HttpRequest currentRequest))
            {
                return default(ITokenDataModel);
            }

            var bearer = TokenUtils.GetTokenFromHeaderOrQuery(currentRequest);
            if (bearer.HasValue())
            {
                var cacheKey = bearer.ToMD5();
                var cached = await m_CacheClient.GetAsync<TokenDataModel>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }

                var tokenModel = await TokenUtils.GetTokenDetail(request?.OriginalRequest as HttpRequest);
                if (null != tokenModel)
                {
                    _ = m_CacheClient.SetAsync(cacheKey, tokenModel, (int)TimeSpan.FromHours(1).TotalSeconds);
                    return tokenModel;
                }
            }

            var apiKey = TokenUtils.GetApiKeyFromHeaderOrQuery(currentRequest);
            if (apiKey.HasValue())
            {
                var cacheKey = apiKey.ToMD5();
                var cached = await m_CacheClient.GetAsync<TokenDataModel>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }

                var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
                if (null != profile)
                {
                    var tokenModel = new TokenDataModel
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

                    _ = m_CacheClient.SetAsync(cacheKey, tokenModel, (int)TimeSpan.FromHours(1).TotalSeconds);
                    return tokenModel;
                }
            }

            return default(ITokenDataModel);
        }

        private static readonly ISerializer m_Serializer;
        private static readonly ICache m_CacheClient;
    }
}
