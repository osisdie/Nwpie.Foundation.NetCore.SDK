using System;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Models
{
    public interface ICObject
    {
        /// <summary>
        /// Microsoft.Extensions.Logging Interface
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// GUID auto gen
        /// </summary>
        Guid _id { get; }

        /// <summary>
        /// DateTime.UtcNow
        /// </summary>
        DateTime _ts { get; }
    }
}
