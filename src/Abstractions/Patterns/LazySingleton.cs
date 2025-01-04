using System;
using System.Threading;
using Nwpie.Foundation.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Patterns
{
    /// <summary>
    /// Lazy Singleton (abstract base class)
    /// </summary>
    /// <typeparam name="T">Generic instance type</typeparam>
    public abstract class LazySingleton<T>
        where T : LazySingleton<T>, new()
    {
        protected LazySingleton()
        {
            Logger = LogMgr.CreateLogger(GetType());
            Logger.LogTrace($"Initializing...");

            Initialization();
        }

        private void Initialization()
        {
            // Guarantee InitialInConstructor() method is called ONLY once
            if (MaxInitializedCount == Interlocked.Add(ref m_CurrentInitializedCount, MaxInitializedCount))
            {
                InitialInConstructor();
            }
        }

        protected abstract void InitialInConstructor();

        public static T Instance => LazyInstance.Value;

        /// <summary>
        /// Interlocked maximum initialized number
        /// </summary>
        private const int MaxInitializedCount = 1;

        /// <summary>
        /// Lazily creates instance
        /// </summary>
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(() => new T());

        /// <summary>
        /// Current initialized count number
        /// </summary>
        private int m_CurrentInitializedCount = 0;

        public ILogger Logger { get; private set; }
        public Guid _id { get; private set; } = Guid.NewGuid();
        public DateTime _ts { get; private set; } = DateTime.UtcNow;
    }
}
