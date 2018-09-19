using GalaSoft.MvvmLight.Ioc;
using SixNations.API.Constants;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.Modules;
using SixNations.Data.Services;
using System.Collections.Generic;

namespace SixNations.CLI
{
    sealed class Kernel
    {
        private readonly IEnumerable<IModule> _modules;

        public Kernel(string[] args)
        {
            SetupModules();
            SetupSubModules();
            SetupServices();
            _modules = Router.Route(args);
        }       

        public async void Run()
        {
            Feedback.Splash();

            foreach (var module in _modules)
            {
                await module.RunAsync();
            }
        }

        private void SetupModules()
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
        }

        private void SetupSubModules()
        {
            SimpleIoc.Default.Register<Login>();
            SimpleIoc.Default.Register<ISubModule>(
                () => SimpleIoc.Default.GetInstance<Login>(), nameof(Login));
        }

        private void SetupServices()
        {
            if (!SimpleIoc.Default.IsRegistered<IAuthTokenService>())
            {
                if (Props.ApiBaseURL == Props.MOCKED)
                {
                    SimpleIoc.Default.Register<IAuthTokenService>(() =>
                    {
                        return new MockedAuthTokenService();
                    });
                }
                else
                {
                    SimpleIoc.Default.Register<IAuthTokenService>(() =>
                    {
                        return new AuthTokenService();
                    });
                }
            }
        }
    }
}
