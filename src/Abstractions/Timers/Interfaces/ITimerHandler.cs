namespace Nwpie.Foundation.Abstractions.Timers.Interfaces
{
    public interface ITimerHandler
    {
        void Initialization();
        void Start();
        bool IsRunning();
        void Pause();
        void Resume();
        void OnExecuting(object state);
        int Interval { get; set; }
    }
}
