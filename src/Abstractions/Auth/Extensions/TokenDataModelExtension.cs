using Nwpie.Foundation.Abstractions.Auth.Interfaces;

namespace Nwpie.Foundation.Abstractions.Auth.Extensions
{
    public static class TokenDataModelExtension
    {
        // AccountId first
        public static string GetRequester(this ITokenDataModel tokenModel) =>
            tokenModel?.AccountId ?? tokenModel?.ApiKey ?? tokenModel?.Name;

        public static string GetAccountLevel(this ITokenDataModel tokenModel) =>
            (tokenModel?.LV ?? 0).ToString();

        public static string GetAccountId(this ITokenDataModel tokenModel) =>
            tokenModel?.AccountId ?? tokenModel?.ApiKey;

        public static string GetAccountName(this ITokenDataModel tokenModel) =>
            tokenModel?.Name;

        public static string GetApiKey(this ITokenDataModel tokenModel) =>
            tokenModel?.ApiKey;
    }
}
