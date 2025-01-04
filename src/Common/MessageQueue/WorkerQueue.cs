using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Common.MessageQueue
{
    /// <summary>
    /// An auto-flush queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WorkerQueue<T> : CObject, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Init WorkerQueue object which will flush automatically by given triggers, maxQueueSize or flushInterval;
        /// if queue size >= maxQueueSize, or time since last flush reach flushInterval, a flush happens, and Flush event fires.
        /// when flush occurs, queue will be clear, all queued items will be gathered, and passed as the 2nd parameter of Flush event.
        /// The Max Queue lenght is 10w, if items more then queue Max Length, it will be lost.
        /// if flushInterval less then 25ms, flushInterval will be 25ms
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="flushInterval">in ms</param>

        public WorkerQueue(int batchSize, int flushInterval, string queueName = "")
            : base()
        {
            Init(batchSize, flushInterval, DefaultMaxQueueCapacity, queueName);
        }

        /// <summary>
        /// Init WorkerQueue object which will flush automatically by given triggers, maxQueueSize or flushInterval;
        /// if queue size >= maxQueueSize, or time since last flush reach flushInterval, a flush happens, and Flush event fires.
        /// when flush occurs, queue will be clear, all queued items will be gathered, and passed as the 2nd parameter of Flush event.
        /// The Max Queue lenght is 10w, if items more then queue Max Length, it will be lost.
        /// if flushInterval less then 25ms, flushInterval will be 25ms
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="flushInterval"></param>
        /// <param name="queueCapacity">The max length for queue, if items more then queue Max Length, it will be lost.</param>
        public WorkerQueue(int batchSize, int flushInterval, int queueCapacity, string queueName = "")
            : base()
        {
            Init(batchSize, flushInterval, queueCapacity, queueName);
        }

        private void Init(int batchSize, int flushInterval, int maxQueueLength, string queueName = "")
        {
            m_InnerQueue = new ConcurrentQueue<T>();
            m_MaxBatchDequeueSize = batchSize.AssignIfNotSet(DefaultMaxBatchDequeueSize);
            m_FlushInterval = flushInterval.AssignIfNotSet(DefaultFlushInterval);
            m_MaxQueueLength = maxQueueLength.AssignIfNotSet(DefaultMaxQueueCapacity);
            QueueName = string.IsNullOrWhiteSpace(queueName)
                ? "DefaultWorkerQueue"
                : queueName;

            m_ProcessQueueTask = Task.Factory.StartNew(ProcessQueue);
            StartRepeatFlushQueueTimer();
        }

        #endregion

        #region Queue
        public void Enqueue(T item)
        {
            try
            {
                if (Length < Capacity)
                {
                    m_InnerQueue.Enqueue(item);
                    if (Length > m_MaxBatchDequeueSize)
                    {
                        SetDequeueEvent();
                    }
                }
                else
                {
                    Logger.LogTrace($"WorkerQueue: Queue full, there are {Length} items in queue.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// De queue all Item, only base on invoke time
        /// </summary>
        public void DequeueAll()
        {
            while (false == m_InnerQueue.IsEmpty)
            {
                BatchDequeue(m_MaxBatchDequeueSize);
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                BatchDequeue(m_MaxBatchDequeueSize);
            }
        }

        #endregion

        #region private method

        private void ProcessQueue()
        {
            while (true)
            {
                WaitDequeueEvent();
                DequeueAll();
            }
        }

        private void BatchDequeue(int batchSize)
        {
            var items = new List<T>();
            while (items.Count < batchSize && Length > 0)
            {
                if (m_InnerQueue.TryDequeue(out var item))
                {
                    items.Add(item);
                }
            }

            OnFlush(items);
        }

        private void StartRepeatFlushQueueTimer()
        {
            m_Timer = new Timer(TimerCallback,
                null,
                m_FlushInterval,
                Timeout.Infinite
            );
        }

        private void TimerCallback(object state)
        {
            SetDequeueEvent();
            RestartTimer();
        }

        private void RestartTimer()
        {
            m_Timer?.Change(m_FlushInterval, Timeout.Infinite);
        }

        private void OnFlush(List<T> items)
        {
            try
            {
                if (items?.Count > 0)
                {
                    Flush?.Invoke(this, items);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private void SetDequeueEvent() =>
            Interlocked.Exchange(ref m_DequeueFlag, 1);

        private void WaitDequeueEvent()
        {
            while (true)
            {
                if (1 == Interlocked.Exchange(ref m_DequeueFlag, 0))
                {
                    return;
                }

                Thread.Sleep(m_FlushInterval);
            }
        }
        #endregion

        public string QueueName { get; private set; }
        public int Length
        {
            get => m_InnerQueue?.Count ?? 0;
        }

        public int Capacity
        {
            get => m_MaxQueueLength;
        }

        public const int DefaultMaxQueueCapacity = 100000;
        public const int DefaultMaxBatchDequeueSize = 100;
        public const int DefaultFlushInterval = 1000;

        /// <summary>
        /// The Timer class has the same resolution as the system clock.
        /// This means that if the period is less than the resolution of the system clock,
        /// the TimerCallback delegate will execute at intervals defined by the resolution of the system clock,
        /// which is approximately 15 milliseconds on Windows 7 and Windows 8 systems.
        /// </summary>
        public const int SystemClockResolution = 15;
        public event Action<object, List<T>> Flush;

        /// <summary>
        /// Event occurs when queue size exceed maxQueueSize,
        /// or flushInterval reached.
        /// You can get queued items from the 2nd parameter of type List<T>.
        /// </summary>
        ConcurrentQueue<T> m_InnerQueue;
        Task m_ProcessQueueTask;
        int m_MaxBatchDequeueSize = DefaultMaxBatchDequeueSize;
        int m_MaxQueueLength = DefaultMaxQueueCapacity;
        int m_FlushInterval = DefaultFlushInterval;
        Timer m_Timer;
        int m_DequeueFlag = 0;
    }
}
