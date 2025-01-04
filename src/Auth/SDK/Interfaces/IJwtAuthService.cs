using Nwpie.Foundation.Abstractions.Auth.Interfaces;

namespace Nwpie.Foundation.Auth.SDK.Interfaces
{
    public interface IJwtAuthService : ITokenService
    {
    }

    public interface IJwtAuthService<T> : ITokenService<T>
        where T : class, ITokenDataModel, new()
    {
    }
}
