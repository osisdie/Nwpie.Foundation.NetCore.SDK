using Microsoft.Extensions.Options;

namespace Nwpie.Foundation.Abstractions.Config.Interfaces
{
    public interface IConfigOptions<T> : IOptions<T>, IConfigOptions
        where T : class, new()
    {
    }

    public interface IConfigOptions
    {

    }
}
