using GalaSoft.MvvmLight.Ioc;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class Main : BaseModule, IModule
    {
        public void Run()
        {
            var login = SimpleIoc.Default.GetInstance<Login>();
            login.Run();
        }
    }
}
