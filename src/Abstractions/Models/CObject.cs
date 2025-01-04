using System;
using Nwpie.Foundation.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Models
{
    public abstract class CObject : ICObject
    {
        public CObject()
        {
            Logger = LogMgr.CreateLogger(GetType());
        }

        /// <summary>
        /// Microsoft.Extensions.Logging Interface
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// GUID auto gen
        /// </summary>
        public Guid _id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// DateTime.UtcNow
        /// </summary>
        public DateTime _ts { get; private set; } = DateTime.UtcNow;
    }
}
