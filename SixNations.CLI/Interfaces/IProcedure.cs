using System.Threading.Tasks;

namespace SixNations.CLI.Interfaces
{
    public interface IProcedure
    {
        void Run();

        Task RunAsync();
    }
}