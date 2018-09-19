using GalaSoft.MvvmLight.Ioc;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.Modules;
using System.Collections.Generic;

namespace SixNations.CLI
{
    sealed class Kernel
    {
        private readonly IEnumerable<IModule> _modules;

        public Kernel(string[] args)
        {
            SimpleIoc.Default.Register<Main>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<Main>(), nameof(Main));

            SimpleIoc.Default.Register<About>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<About>(), nameof(About));

            SimpleIoc.Default.Register<Help>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<Help>(), nameof(Help));

            SimpleIoc.Default.Register<Login>();
            SimpleIoc.Default.Register<ISubModule>(
                () => SimpleIoc.Default.GetInstance<Login>(), nameof(Login));

            _modules = Router.Route(args);
        }       

        public void Run()
        {
            Feedback.Splash();

            foreach (var module in _modules)
            {
                module.Run();
            }
        }
    }
}
