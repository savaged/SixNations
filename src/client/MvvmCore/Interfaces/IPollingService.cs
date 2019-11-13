namespace savaged.mvvm.Core.Interfaces
{
    public interface IPollingService
    {
        void Kill();
        void Start();
        void Stop();
    }
}