using System;
using System.Threading.Tasks;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class HelpMod : BaseModule, IModule
    {
        public async Task RunAsync()
        {
            Feedback.Show("You're on your own! TODO ;)");
            await Task.CompletedTask;
        }
    }
}
