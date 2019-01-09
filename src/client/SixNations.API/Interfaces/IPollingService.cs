using System;

namespace SixNations.API.Interfaces
{
    public interface IPollingService
    {
        void Kill();
        void Start();
        void Stop();
        void SetInterval(int milliseconds);

        event Action IntervalElapsed;
    }
}