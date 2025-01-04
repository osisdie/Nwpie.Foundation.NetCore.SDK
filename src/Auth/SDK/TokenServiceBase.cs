using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.Auth.SDK
{
    public abstract class TokenServiceBase<T> : CObject, ITokenService
        where T : class, ITokenDataModel, new()
    {
        /// <summary>
        /// Recommend DI singleton for an application
        /// And you can choose your cache provider
        /// </summary>
        /// <param name="cache"></param>
        public TokenServiceBase(IConfigOptions<Auth_Option> option, ISerializer serializer, ICache cache)
        {
            m_Option = option
                ?? throw new ArgumentNullException(nameof(IConfigOptions<Auth_Option>));

            Serializer = serializer
                ?? throw new ArgumentNullException(nameof(ISerializer));

            if (null == m_Option.Value)
            {
                throw new ArgumentNullException(nameof(option.Value));
            }

            CacheClient = cache;
            DefaultCacheDurationSecs = 60 * option.Value.CacheMinutes.AssignIf(x => x <= 1, 5);
            m_LocationClient = ComponentMgr.Instance.TryResolve<ILocationClient>();
            Task.Run(() => WarnUp());
        }

        private async Task WarnUp()
        {
            m_AuthBaseUrl = ServiceContext.AuthServiceUrl;
            if (string.IsNullOrWhiteSpace(m_AuthBaseUrl))
            {
                m_AuthBaseUrl = await m_LocationClient
                    .GetApiLocation(AuthServiceConfig.ServiceName);
            }

            if (null == CacheClient)
            {
                CacheClient = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            }

            // HealthCheck
            try
            {
                // Test
                var cacheKey = $"{ServiceContext.ApiName}-cache-{CacheClient.GetType().Name}-healthcheck".ToLower();
                var setResult = await CacheClient.SetAsync(cacheKey, DateTime.UtcNow.ToString("s"), 10).ConfigureAwait(false);
                if (true == setResult?.IsSuccess)
                {
                    var getResult = await CacheClient.GetAsync<string>(cacheKey).ConfigureAwait(false);
                    if (getResult.Any())
                    {
                        Logger.LogInformation($"{CacheClient.GetType().Name} is healthy and servering {GetType().Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.ToString());
                CacheClient = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            }
        }

        public abstract Task<TModel> GetTokenDetail<TModel>(HttpRequest request)
            where TModel : class, ITokenDataModel, new();

        public abstract Task<string> Encode(ITokenDataModel model);
        public abstract Task<string> Decode(string encrypted);

        public virtual async Task<string> RenewToken(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return string.Empty;
            }

            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                return encrypted;
            }

            var content = await Deserialize<TokenDataModel>(encrypted);
            if (null == content)
            {
                return string.Empty;
            }

            // Refresh access_token
            content.Exp = DateTime.UtcNow.AddMinutes(m_Option.Value.AccessTokenMinutes);
            content.Upt = DateTime.UtcNow;

            return await Encode(content);
        }

        public virtual Task<bool?> IsAdmin(HttpRequest request) =>
            IsAdmin(GetAuthorizationString(request));

        public virtual async Task<bool?> IsAdmin(string encrypted)
        {
            await Task.CompletedTask;
            return encrypted.HasValue() &&
                m_Option.Value.AdminTokenEnabled &&
                m_Option.Value.AdminToken == encrypted;
        }

        public virtual async Task<TModel> GenerateAdminTokenModel<TModel>(
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TokenLevelEnum level = TokenLevelEnum.ApplicationUser)
            where TModel : class, ITokenDataModel, new()
        {
            var now = DateTime.UtcNow;
            var creator = $"{GetType().Name}{__}{m_Option.Value.MinimumAccessTokenVersion}";
            var expireAt = now.AddMinutes(m_Option.Value.AccessTokenMinutes);
            if (TokenKindEnum.RefreshToken == kind)
            {
                creator = $"{GetType().Name}{__}{m_Option.Value.MinimumRefershTokenVersion}";
                expireAt = now.AddMinutes(m_Option.Value.RefreshTokenMinutes);
            }

            var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(ServiceContext.ApiKey);
            if (null == profile?.AccountId)
            {
                return default;
            }

            return new TModel()
            {
                AccountId = profile.AccountId,
                Name = profile.Name,
                Mob = false,
                Kind = (byte)kind,
                LV = (byte)level,
                Iss = creator,
                Exp = expireAt,
                Upt = now,
                Iat = now
            };
        }

        public virtual Task<bool?> IsGeneratedFromAuthServer(HttpRequest request) =>
            IsGeneratedFromAuthServer(GetAuthorizationString(request));

        public virtual async Task<bool?> IsGeneratedFromAuthServer(string encrypted)
        {
            var decoded = await Decode(encrypted);
            return decoded.HasValue();
        }

        public virtual Task<bool?> IsExpired(HttpRequest request) =>
            IsExpired(GetAuthorizationString(request));

        public virtual async Task<bool?> IsExpired(string encrypted)
        {
            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                return false;
            }

            var content = await Deserialize<TokenDataModel>(encrypted);
            if (null == content)
            {
                return null;
            }

            return await IsExpired(content);
        }

        public virtual async Task<bool?> IsExpired(ITokenDataModel tokenModel)
        {
            if (false == tokenModel.Exp.HasValue)
            {
                return true;
            }

            var isExpired = DateTime.UtcNow >= tokenModel.Exp;
            //if (tokenModel.Exp.HasValue)
            //{
            //    isExpired = DateTime.UtcNow >= tokenModel.Exp;
            //}
            //else if (tokenModel.Upt.HasValue)
            //{
            //    isExpired = (DateTime.UtcNow - tokenModel.Upt)
            //        ?.TotalMinutes > m_Option.Value.ExpireMinutes ||
            //         DateTime.UtcNow.CompareTo(tokenModel.Upt) < 0;
            //}

            // check token version
            if (false == isExpired &&
                m_Option.Value.MinimumRefershTokenVersion > 0 &&
                true == tokenModel.Iss?.Contains(__))
            {
                var version = tokenModel.Iss
                    ?.Split(new string[] { __ }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Last();
                if (version.HasValue() &&
                    int.TryParse(version, out var sourceVersion) &&
                    m_Option.Value.MinimumRefershTokenVersion > sourceVersion)
                {
                    isExpired = true;
                }
            }

            await Task.CompletedTask;
            return isExpired;
        }

        public virtual async Task<IServiceResponse<TModel>> VerifyToken<TModel>(
            string encrypted,
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TModel compareToCurrent = null)
            where TModel : class, ITokenDataModel, new()
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.InvalidAccessToken, $"Missing {kind}. ");
            }

            encrypted = encrypted.TrimBearer();
            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return new ServiceResponse<TModel>(true).Content(adminToken);
                }
            }

            var content = await Deserialize<TModel>(encrypted);
            if (null == content)
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.InvalidAccessToken, $"Invalid {kind}. ");
            }

            // Is valid token kind
            if (content.Kind > 0 && (byte)kind != content.Kind)
            {
                return new ServiceResponse<TModel>()
                    .Content(content)
                    .Error(StatusCodeEnum.InvalidAccessToken, $"Unknown token kind(={kind}). ");
            }

            if (true == await IsClientSourceChanged(content, compareToCurrent))
            {
                return new ServiceResponse<TModel>()
                    .Content(content)
                    .Error(StatusCodeEnum.PermissionDeny, "Client's source seems changed. ");
            }

            if (true == await IsExpired(encrypted))
            {
                return new ServiceResponse<TModel>()
                    .Content(content)
                    .Error(StatusCodeEnum.AccessTokenExpired, $"Expired {kind}. ");
            }

            return new ServiceResponse<TModel>(true).Content(content);
        }

        public virtual async Task<bool?> IsClientSourceChanged(ITokenDataModel src, ITokenDataModel current)
        {
            if (null == src || null == current)
            {
                return false;
            }

            var isChanged = false;
            if (true == src.Mob)
            {
                // requrie same Mac Address
                if (src.Mac.HasValue() && current.Mac.HasValue() &&
                    src.Mac != current.Mac)
                {
                    isChanged = true;
                }

                // requrie same User Agent
                //if (src.UA.HasValue() && current.UA.HasValue() &&
                //    false == src.UA.Equals(current.UA.ToMD5(), StringComparison.OrdinalIgnoreCase))
                //{
                //    isChanged = true;
                //}
            }
            else
            {
                // require same IP Address
                if (src.IP.HasValue() && current.IP.HasValue() &&
                    false == (src.IP.Contains("::") && (current.IP?.Contains("::") ?? true)) &&
                    src.IP != current.IP)
                {
                    isChanged = true;
                }

                // requrie same User Agent
                //if (src.UA.HasValue() && current.UA.HasValue() &&
                //    false == src.UA.Equals(current.UA.ToMD5(), StringComparison.OrdinalIgnoreCase))
                //{
                //    isChanged = true;
                //}
            }

            await Task.CompletedTask;
            return isChanged;
        }

        public virtual Task<string> RenewToken(HttpRequest request) =>
            RenewToken(GetAuthorizationString(request));

        /// <summary>
        /// ex:
        /// - Authorization: Bearer {token}
        /// - ?token={token}
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string GetAuthorizationString(HttpRequest request) =>
            TokenUtils.GetTokenFromHeaderOrQuery(request);

        public virtual async Task<TModel> Deserialize<TModel>(string encrypted)
        {
            var decrypted = await Decode(encrypted);
            if (string.IsNullOrWhiteSpace(decrypted))
            {
                return default;
            }

            try
            {
                return Serializer.Deserialize<TModel>(decrypted);
            }
            catch { return default; }
        }

        public virtual async Task<int?> GetMinimumRefershTokenVersion()
        {
            await Task.CompletedTask;
            return m_Option?.Value?.MinimumRefershTokenVersion;
        }

        public virtual Task IncreseMinimumRefershTokenVersion()
        {
            if (null != m_Option?.Value)
            {
                ++m_Option.Value.MinimumRefershTokenVersion;
            }

            return Task.CompletedTask;
        }

        public virtual void Dispose() { }

        public const string __ = CommonConst.Separator;
        public string AdminMails { get; set; }
        public string AdminAccountId { get; set; }
        public int DefaultCacheDurationSecs { get; set; } = 5 * 60; // 5 minutes

        public readonly ISerializer Serializer;
        public ICache CacheClient;

        protected readonly ILocationClient m_LocationClient;
        protected readonly IConfigOptions<Auth_Option> m_Option;
        protected string m_AuthBaseUrl;
    }
}
