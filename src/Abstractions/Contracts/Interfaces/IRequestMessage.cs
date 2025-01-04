using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IRequestMessage
    {
        Dictionary<string, string> ExtensionMap { get; set; }
        bool Validate();
        bool ValidateAndThrow();
    }

    public interface IRequestMessage<T> : IRequestMessage
    {
        T Data { get; set; }
    }
}
