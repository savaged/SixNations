using System.Threading.Tasks;

namespace SixNations.CLI.Interfaces
{
    public interface IModule : IProcedure
    {
        string Name { get; }
    }
}