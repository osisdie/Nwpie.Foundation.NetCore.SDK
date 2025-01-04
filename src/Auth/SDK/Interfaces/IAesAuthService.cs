using Nwpie.Foundation.Abstractions.Auth.Interfaces;

namespace Nwpie.Foundation.Auth.SDK.Interfaces
{
    public interface IAesAuthService : ITokenService
    {
    }

    public interface IAesAuthService<T> : ITokenService<T>
        where T : class, ITokenDataModel, new()
    {
    }
}
