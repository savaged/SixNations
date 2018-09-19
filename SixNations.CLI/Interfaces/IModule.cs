using System.Threading.Tasks;

namespace SixNations.CLI.Interfaces
{
    public interface IModule
    {
        void Run();

        Task RunAsync();

        string Name { get; }
    }
}