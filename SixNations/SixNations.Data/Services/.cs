namespace SixNations.Api.Interfaces
{
    public interface IKeepAuthAliveService
    {
        void Kill();
        void Start();
        void Stop();
    }
}