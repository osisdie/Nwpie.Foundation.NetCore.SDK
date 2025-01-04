using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Auth.SDK.Providers
{
    public class DefaultJwtAuthService<T> : TokenServiceBase<T>, IJwtAuthService
        where T : TokenDataModel, new()
    {
        /// <summary>
        /// Recommend DI singleton for an application
        /// And you can choose your cache provider
        /// </summary>
        /// <param name="cache"></param>
        public DefaultJwtAuthService(IConfigOptions<Auth_Option> option, ISerializer serializer, ICache cache)
            : base(option, serializer, cache)
        {
            if (string.IsNullOrWhiteSpace(m_Option.Value.JwtPrivateKey))
            {
                throw new ArgumentNullException(nameof(Auth_Option.JwtPrivateKey));
            }

            if (string.IsNullOrWhiteSpace(m_Option.Value.JwtAlgorithm) ||
                JwtHashAlgorithmEnum.UnSet == Enum<JwtHashAlgorithmEnum>.TryParse(m_Option.Value.JwtAlgorithm, JwtHashAlgorithmEnum.UnSet))
            {
                throw new ArgumentNullException(nameof(Auth_Option.JwtAlgorithm));
            }
        }

        public override async Task<TModel> GetTokenDetail<TModel>(HttpRequest request)
        {
            if (null == request)
            {
                return default(TModel);
            }

            var authToken = GetAuthorizationString(request);
            if (string.IsNullOrWhiteSpace(authToken))
            {
                return default(TModel);
            }

            var encrypted = authToken.TrimBearer();
            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return adminToken;
                }
            }

            return await Deserialize<TModel>(encrypted);
        }

        public override Task<bool?> IsGeneratedFromAuthServer(string encrypted) =>
            base.IsGeneratedFromAuthServer(TokenUtils.TrimBearer(encrypted));

        public override Task<bool?> IsExpired(string encrypted) =>
            base.IsExpired(TokenUtils.TrimBearer(encrypted));

        public override Task<bool?> IsAdmin(string encrypted) =>
            base.IsAdmin(TokenUtils.TrimBearer(encrypted));

        public override async Task<string> Encode(ITokenDataModel model)
        {
            string encoded = null;
            try
            {
                encoded = JwtUtils.Encode(model,
                    m_Option.Value.JwtPrivateKey,
                    Enum<JwtHashAlgorithmEnum>.TryParse(m_Option.Value.JwtAlgorithm, JwtUtils.DefaultJwtAlgorithm)
                );
            }
            catch { }

            await Task.CompletedTask;
            return encoded;
        }

        public override async Task<string> Decode(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return null;
            }

            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                var adminToken = await GenerateAdminTokenModel<TokenDataModel>();
                if (null != adminToken?.AccountId)
                {
                    return Serializer.Serialize(adminToken);
                }
            }

            var cacheKey = $"{ServiceContext.ApiName}{__}Token{__}{encrypted.ToMD5()}".ToLower();
            if (null != CacheClient)
            {
                var cached = await CacheClient.GetAsync<string>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    return cached.Data;
                }
            }

            try
            {
                encrypted = TokenUtils.TrimBearer(encrypted);
                var result = JwtUtils.Decode<string>(encrypted, m_Option.Value.JwtPrivateKey, verify: true);
                if (result.IsSuccess && result.Data.HasValue())
                {
                    if (null != CacheClient)
                    {
                        _ = CacheClient.SetAsync(cacheKey, result.Data, DefaultCacheDurationSecs);
                    }

                    return result.Data;
                }

                throw new Exception(result.ErrMsg);
            }
            catch { return null; }
        }

        public override void Dispose() { }
    }
}
