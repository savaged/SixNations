using System;
using System.Threading.Tasks;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class About : BaseModule, IModule
    {
        private void Run()
        {
            var about = new Data.Models.About();
            Feedback.Show(about);
        }

        public async Task RunAsync()
        {
            Run();
            await Task.CompletedTask;
        }
    }
}
