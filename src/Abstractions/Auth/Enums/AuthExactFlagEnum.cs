namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    public enum AuthExactFlagEnum
    {
        Unset = 0,
        AuthorizationHeader = 1,
        TokenQueryString = 2,
        PAT_Header = 4,
        ApiKeyHeader = 16,
        ApiKeyInToken = 32,

        Max = 512
    }
}
