using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using SixNations.CLI.Interfaces;

namespace SixNations.CLI.Modules
{
    public class Main : BaseModule, IModule
    {
        public void Run()
        {
            throw new NotSupportedException();
        }

        public async Task RunAsync()
        {
            await SimpleIoc.Default.GetInstance<Login>().RunAsync();
            // TODO more logic here ;)
        }
    }
}
