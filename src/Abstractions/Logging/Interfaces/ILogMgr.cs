using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Logging.Interfaces
{
    public interface ILogMgr : ISingleCObject
    {
        ILoggerFactory LoggerFactory { get; }

        ILogger CreateLogger<T>();
    }
}
