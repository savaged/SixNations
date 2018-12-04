namespace SixNations.API.Interfaces
{
    public interface IKeepAuthAliveService
    {
        void Kill();
        void Start();
        void Stop();
    }
}