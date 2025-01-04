using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IServiceConfig
    {
        string ServiceName { get; }
        List<string> Tags { get; }
    }
}
