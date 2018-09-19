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
            SimpleIoc.Default.GetInstance<Login>().Run();
            // TODO more logic here ;)
        }

        public Task RunAsync()
        {
            throw new NotSupportedException();
        }
    }
}
