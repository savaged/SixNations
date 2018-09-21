using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class BaseModule
    {
        internal IInputEntryService Entry { get; }

        public BaseModule()
        {
        }

        public BaseModule(IInputEntryService entryService)
        {
            Entry = entryService;
        }

        public string Name => GetType().Name;
    }
}
