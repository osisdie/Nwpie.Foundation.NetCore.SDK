using System;
using System.Threading;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Timers.Interfaces;
using Nwpie.Foundation.Abstractions.Utilities;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Abstractions.Timers
{
    public abstract class TimerHandlerBase : CObject, ITimerHandler
    {
        public TimerHandlerBase(int interval)
        {
            Interval = interval;
            Init();
        }

        void Init()
        {
            m_FailbackScore = FailbackScoreInfo.CreateNew;

            Initialization();
        }

        public abstract void Initialization();

        public virtual void Start()
        {
            if (null == m_Timer && Interval > 0)
            {
                m_Timer = new Timer(TimerCallback,
                   null,
                   Interval,
                   Timeout.Infinite
               );
            }
        }

        public virtual bool IsRunning() =>
            null != m_Timer && Interval > 0;

        public virtual void Pause()
        {
            OriginalInterval = Interval;
            Interval = 0;
        }

        public virtual void Resume()
        {
            Interval = OriginalInterval;
        }

        void TimerCallback(object state)
        {
            if (m_FailbackScore.IsExceedLimit())
            {
                return;
            }

            try
            {
                OnExecuting(state);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                m_FailbackScore.Fail();
            }

            RestartTimer();
        }

        public void RestartTimer()
        {
            if (Interval > 0)
            {
                m_Timer?.Change(
                    Interval,
                    Timeout.Infinite
                );
            }
        }

        public abstract void OnExecuting(object state);

        public int Interval { get; set; } = 300000; // in milliseconds

        protected int OriginalInterval = 300000; // in milliseconds
        protected Timer m_Timer;
        protected IFailbackScore m_FailbackScore;
    }
}
