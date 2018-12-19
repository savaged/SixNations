namespace SixNations.API.Interfaces
{
    public interface IPollingService
    {
        void Kill();
        void Start();
        void Stop();
    }
}