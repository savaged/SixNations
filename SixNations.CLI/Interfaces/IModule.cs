namespace SixNations.CLI.Interfaces
{
    public interface IModule
    {
        void Run();

        string Name { get; }
    }
}