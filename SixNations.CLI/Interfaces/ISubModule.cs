using System.Threading.Tasks;

namespace SixNations.CLI.Interfaces
{
    public interface ISubModule
    {
        void Run();

        Task RunAsync();
    }
}