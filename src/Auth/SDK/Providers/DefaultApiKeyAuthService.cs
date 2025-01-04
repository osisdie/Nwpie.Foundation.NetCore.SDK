using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Auth.SDK.Providers
{
    public class DefaultApiKeyAuthService<T> :
        TokenServiceBase<T>,
        IApiKeyAuthService
        where T : TokenDataModel, new()
    {
        /// <summary>
        /// Recommend DI singleton for an application
        /// And you can choose your cache provider
        /// </summary>
        /// <param name="cache"></param>
        public DefaultApiKeyAuthService(IConfigOptions<Auth_Option> option, ISerializer serializer, ICache cache)
            : base(option, serializer, cache)
        {

        }

        public override async Task<TModel> GetTokenDetail<TModel>(HttpRequest request)
        {
            if (null == request)
            {
                return default;
            }

            var encryptedToken = GetAuthorizationString(request);
            if (string.IsNullOrWhiteSpace(encryptedToken))
            {
                var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(
                    request,
                    AuthExactFlagEnum.ApiKeyHeader
                );
                if (null == profile)
                {
                    return default;
                }

                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return adminToken;
                }

                return default;
            }

            var isAdmin = await IsAdmin(encryptedToken);
            if (true == isAdmin)
            {
                var adminToken = await GenerateAdminTokenModel<TModel>();
                if (null != adminToken?.AccountId)
                {
                    return adminToken;
                }
            }

            return await Deserialize<TModel>(encryptedToken);
        }

        public override async Task<string> RenewToken(string encrypted)
        {
            await Task.CompletedTask;
            return encrypted;
        }

        public override async Task<bool?> IsAdmin(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return false;
            }

            await Task.CompletedTask;
            return m_Option.Value.AdminTokenEnabled &&
                m_Option.Value.AdminToken == encrypted;
        }

        public override async Task<string> Encode(ITokenDataModel model)
        {
            string encoded = null;
            try
            {
                encoded = Serializer.Serialize(model);
            }
            catch { }

            await Task.CompletedTask;
            return encoded;
        }

        public override async Task<string> Decode(string encrypted)
        {
            await Task.CompletedTask;
            return encrypted;
        }

        public override async Task<bool?> IsClientSourceChanged(ITokenDataModel src, ITokenDataModel current)
        {
            if (null == current)
            {
                return false;
            }

            var isChanged = false;
            if (current.ApiKey.HasValue() &&
                current.ApiKey != src.ApiKey)
            {
                isChanged = true;
            }

            await Task.CompletedTask;
            return isChanged;
        }

        public override async Task<TModel> GenerateAdminTokenModel<TModel>(
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TokenLevelEnum level = TokenLevelEnum.ApplicationUser)
        {
            var now = DateTime.UtcNow;
            var creator = $"{GetType().Name}{__}{m_Option.Value.MinimumApiTokenVersion}";
            var expireAt = now.AddMinutes(m_Option.Value.ApiTokenMinutes);
            var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(ServiceContext.ApiKey);
            if (null == profile?.AccountId)
            {
                return default(TModel);
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

        public override void Dispose() { }
    }
}
