using System;
using System.Threading.Tasks;
using SixNations.CLI.Interfaces;

namespace SixNations.CLI.Modules
{
    public class Help : BaseModule, IModule
    {
        public void Run()
        {
            // TODO
        }

        public Task RunAsync()
        {
            throw new NotSupportedException();
        }
    }
}
