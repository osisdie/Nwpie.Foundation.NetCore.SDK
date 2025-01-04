using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
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
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.S3Proxy.Endpoint.Auth
{
    public class TodoAesTokenService : DefaultAesAuthService<TokenDataModel>
    {
        public TodoAesTokenService(IConfigOptions<Auth_Option> option, ISerializer serializer, ICache cache)
           : base(option, serializer, cache)
        {

        }

        public override async Task<TModel> GenerateAdminTokenModel<TModel>(
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TokenLevelEnum level = TokenLevelEnum.EndAdminUser)
        {
            await Task.CompletedTask;
            return default(TModel);
        }

        public override async Task<IServiceResponse<TModel>> VerifyToken<TModel>(
            string encrypted,
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TModel compareToCurrent = null)
        {
            // auth -> { ip: ip, mac: mac, token: token }
            AuthRequestModel authModel = null;
            var isAdmin = await IsAdmin(encrypted);
            if (true == isAdmin)
            {
                authModel = new AuthRequestModel()
                {
                    Token = encrypted
                };
            }
            else
            {
                try
                {
                    var data = Convert.FromBase64String(encrypted);
                    var decodedString = Encoding.UTF8.GetString(data);
                    authModel = Serializer
                        .Deserialize<AuthRequestModel>(decodedString);
                }
                catch
                {
                    return new ServiceResponse<TModel>()
                        .Error(StatusCodeEnum.InvalidAccessToken, $"Invalid {kind}. ");
                }
            }

            var content = await Deserialize<TModel>(authModel?.Token);
            if (null == content)
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.InvalidAccessToken, $"Invalid {kind}. ");
            }

            // Is valid token kind ?
            //if (content.Kind > 0 && (byte)kind != content.Kind)
            //{
            //    return new ServiceResponse<T>().Error(StatusCodeEnum.AccessToken_Invalid, "Invalid token kind validation. ");
            //}

            if (true == await IsClientSourceChanged(content, compareToCurrent))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.PermissionDeny, "Client source seems changed. ");
            }

            if (true == await IsExpired(encrypted))
            {
                return new ServiceResponse<TModel>()
                    .Error(StatusCodeEnum.AccessTokenExpired, $"Expired {kind}. ");
            }

            return new ServiceResponse<TModel>(true).Content(content);
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

            var isAdmin = await IsAdmin(authToken);
            if (true == isAdmin)
            {
                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return adminToken;
                }
            }

            var authModel = DeserizeAuthorization(authToken);
            if (true == authModel?.Token.HasValue())
            {
                var tokenModel = await Deserialize<TModel>(authModel.Token);
                tokenModel.ExtensionMap = tokenModel.ExtensionMap.AssignIfNotSet(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                tokenModel.ExtensionMap[CommonConst.AuthHeaderName] = authToken;
                tokenModel.ExtensionMap[CommonConst.TokenPropertyName] = authModel.Token;

                return tokenModel; // authModel;
            }

            return null;
        }

        public override async Task<string> RenewToken(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return null;
            }

            if (true == await IsAdmin(encrypted))
            {
                return string.Empty;
            }

            var authModel = DeserizeAuthorization(encrypted);
            if (null != authModel?.Token)
            {
                var tokenModel = await Deserialize<TokenDataModel>(authModel.Token);
                if (null != tokenModel)
                {
                    // TODO: refresh access_token instead of refresh_token
                    tokenModel.Exp = DateTime.UtcNow.AddDays(1); // a day
                    tokenModel.Upt = DateTime.UtcNow;

                    return await Encode(tokenModel);
                }
            }

            return string.Empty;
        }

        public override async Task<bool?> IsGeneratedFromAuthServer(string encrypted)
        {
            var authModel = DeserizeAuthorization(encrypted);
            await Task.CompletedTask;
            return true == authModel?.Token.HasValue();
        }

        protected AuthRequestModel DeserizeAuthorization(string encrypted)
        {
            try
            {
                var data = Convert.FromBase64String(encrypted);
                var decodedString = Encoding.UTF8.GetString(data);

                return Serializer.Deserialize<AuthRequestModel>(decodedString);
            }
            catch { return null; }
        }

        public override async Task<string> Decode(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return null;
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

        public override async Task<TModel> Deserialize<TModel>(string encrypted)
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

        public override async Task<bool?> IsExpired(string encryptedToken)
        {
            var authModel = DeserizeAuthorization(encryptedToken);
            if (false == string.IsNullOrWhiteSpace(authModel?.Token))
            {
                var content = await Deserialize<TokenDataModel>(authModel.Token);
                if (null == content)
                {
                    return null;
                }

                return await IsExpired(content);
            }

            return null;
        }

        public override async Task<bool?> IsAdmin(string encrypted)
        {
            await Task.CompletedTask;
            return false;
        }
    }
}
