using Nwpie.Foundation.Abstractions.Auth.Interfaces;

namespace Nwpie.Foundation.Auth.SDK.Interfaces
{
    public interface IApiKeyAuthService : ITokenService
    {
    }

    public interface IApiKeyAuthService<T> : ITokenService<T>
        where T : class, ITokenDataModel, new()
    {
    }
}
