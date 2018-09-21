namespace SixNations.CLI.IO
{
    public interface IInputEntryService
    {
        string Read(string label, bool masked = false);
    }
}