using System;

namespace SixNations.CLI.IO
{
    public interface IInputEntryService
    {
        string Read(string label, bool masked = false);

        string ReadMenu(string menu);

        event EventHandler<EventArgs> Escaped;
    }
}