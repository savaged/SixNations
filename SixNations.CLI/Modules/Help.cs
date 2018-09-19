using System;
using System.Threading.Tasks;
using SixNations.CLI.Interfaces;

namespace SixNations.CLI.Modules
{
    public class Help : BaseModule, IModule
    {
        public Task RunAsync()
        {
            throw new NotSupportedException();
        }
    }
}
