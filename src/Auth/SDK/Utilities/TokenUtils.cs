using System.Threading.Tasks;
using System.Web;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Auth.SDK.Utilities
{
    public static class TokenUtils
    {
        static TokenUtils()
        {
            Serializer = new DefaultSerializer();
        }

        public static string GetRequester(HttpRequest request)
        {
            var detail = GetTokenDetail(request).ConfigureAwait(false).GetAwaiter().GetResult();
            if (null != detail)
            {
                return detail.GetRequester();
            }

            return null;
        }

        public static async Task<ITokenDataModel> GetTokenDetail(HttpRequest request)
        {
            var svc = ComponentMgr.Instance.GetDefaultJwtTokenService();
            if (null == svc)
            {
                return default(ITokenDataModel);
            }

            return await svc.GetTokenDetail<TokenDataModel>(request);
        }

        public static async Task<string> ExtractPAT(HttpRequest request, AuthExactFlagEnum flags)
        {
            string pat = null;
            if (flags.HasFlag(AuthExactFlagEnum.PAT_Header))
            {
                pat = GetPAT_FromHeaderOrQuery(request);
            }

            await Task.CompletedTask;
            return pat;
        }

        public static async Task<string> ExtractApiKey(HttpRequest request, AuthExactFlagEnum flags)
        {
            if (flags.HasFlag(AuthExactFlagEnum.ApiKeyHeader))
            {
                var apiKey = GetApiKeyFromHeaderOrQuery(request);
                if (apiKey.HasValue())
                {
                    return apiKey;
                }
            }

            if (flags.HasFlag(AuthExactFlagEnum.ApiKeyInToken))
            {
                var detail = await GetTokenDetail(request);
                if (null != detail)
                {
                    return detail.GetApiKey();
                }
            }

            return null;
        }

        public static async Task<bool> IsEndUserToken(HttpRequest request)
        {
            var detail = await GetTokenDetail(request);

            return detail?.LV >= (byte)TokenLevelEnum.EndUser &&
                detail?.LV < (byte)TokenLevelEnum.BusinessUser;
        }

        public static async Task<bool> IsApiKeyToken(HttpRequest request)
        {
            var detail = await GetTokenDetail(request);

            return detail?.LV >= (byte)TokenLevelEnum.ApplicationUser &&
                detail?.LV < (byte)TokenLevelEnum.Max;
        }

        public static bool IsActiveToken(this ITokenDataModel model)
        {
            if (null != model && model.AccountId.HasValue())
            {
                return model.IsValid();
            }

            return false;
        }

        public static string GetValueFromHeaderOrQuery(this HttpRequest request, string headerKey, string tokenKey = null)
        {
            var valueInHeader = request?.Headers?[headerKey];
            if (string.IsNullOrWhiteSpace(valueInHeader) &&
                true == request?.QueryString.HasValue)
            {
                valueInHeader = HttpUtility
                    .ParseQueryString(request.QueryString.Value)
                    ?[(tokenKey ?? headerKey).ToLower()];
            }

            return valueInHeader;
        }

        public static string GetPAT_FromHeaderOrQuery(HttpRequest request) =>
            request?.GetValueFromHeaderOrQuery(CommonConst.PAT)
                .AssignIfNotSet(null);

        public static string GetApiKeyFromHeaderOrQuery(HttpRequest request) =>
            request?.GetValueFromHeaderOrQuery(CommonConst.ApiKey)
                .AssignIfNotSet(null);

        public static string GetTokenFromHeaderOrQuery(HttpRequest request) =>
            request?.GetValueFromHeaderOrQuery(CommonConst.AuthHeaderName, CommonConst.TokenPropertyName)
                .AssignIfNotSet(null);

        public static string TrimBearer(string token) =>
            token.TrimBearer();

        public static ISerializer Serializer { get; private set; }
    }
}
