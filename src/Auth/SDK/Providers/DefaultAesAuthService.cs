using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
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
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Auth.SDK.Providers
{
    public class DefaultAesAuthService<T> :
        TokenServiceBase<T>,
        IAesAuthService
        where T : class, ITokenDataModel, new()
    {
        /// <summary>
        /// Recommend DI singleton for an application
        /// And you can choose your cache provider
        /// </summary>
        /// <param name="cache"></param>
        public DefaultAesAuthService(IConfigOptions<Auth_Option> option, ISerializer serializer, ICache cache)
            : base(option, serializer, cache)
        {
            if (string.IsNullOrWhiteSpace(m_Option.Value.AuthAESKey))
            {
                throw new ArgumentNullException(nameof(Auth_Option.AuthAESKey));
            }

            if (string.IsNullOrWhiteSpace(m_Option.Value.AuthAESIV))
            {
                throw new ArgumentNullException(nameof(Auth_Option.AuthAESIV));
            }

            m_CryptoAes = Aes.Create();
            m_CryptoAes.Mode = CipherMode.CBC;
            m_CryptoAes.Padding = PaddingMode.PKCS7;
            m_CryptoAes.Key = Convert.FromBase64String(m_Option.Value.AuthAESKey);
            m_CryptoAes.IV = Convert.FromBase64String(m_Option.Value.AuthAESIV);
        }

        public override async Task<TModel> GetTokenDetail<TModel>(HttpRequest request)
        {
            if (null == request)
            {
                return default(TModel);
            }

            var encrypted = GetAuthorizationString(request);
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return adminToken;
                }

                return default(TModel);
            }

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

        public override async Task<string> Encode(ITokenDataModel model)
        {
            var text = Serializer.Serialize(model);
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            string encoded = null;
            try
            {
                var textBytes = Encoding.UTF8.GetBytes(text);
                var transform = m_CryptoAes.CreateEncryptor();

                encoded = Convert.ToBase64String(transform.TransformFinalBlock(textBytes, 0, textBytes.Length));
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
                var textBytes = CryptoUtils.TryDecodeBase64(encrypted);
                var transform = m_CryptoAes.CreateDecryptor();
                var decrypted = Encoding.UTF8.GetString(transform.TransformFinalBlock(textBytes, 0, textBytes.Length));
                if (decrypted.HasValue() && null != CacheClient)
                {
                    _ = CacheClient.SetAsync(cacheKey, decrypted, DefaultCacheDurationSecs);
                }

                return decrypted;
            }
            catch { return null; }
        }

        public override async Task<IServiceResponse<TModel>> VerifyToken<TModel>(
            string encrypted,
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TModel compareToCurrent = null)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.MissingAccessToken, $"Missing {kind}. ");
            }

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

            if (true == await IsClientSourceChanged(content, compareToCurrent))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.PermissionDeny, "Client's source seems changed. ");
            }

            if (true == await IsExpired(content))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.AccessTokenExpired, $"Expired {kind}. ");
            }

            return new ServiceResponse<TModel>(true).Content(content);
        }

        public override void Dispose()
        {
            m_CryptoAes?.Dispose();
        }

        protected readonly Aes m_CryptoAes;
    }
}
