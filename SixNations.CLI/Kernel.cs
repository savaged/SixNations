using GalaSoft.MvvmLight.Ioc;
using SixNations.API.Constants;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.Modules;
using SixNations.Data.Facade;
using SixNations.Data.Models;
using SixNations.Data.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.CLI
{
    sealed class Kernel
    {
        private readonly IEnumerable<IModule> _modules;

        public Kernel(string[] args)
        {
            SetupServices();
            SetupSubModules();
            SetupModules();
            _modules = Router.Route(args);
        }       

        public async Task RunAsync()
        {
            Feedback.Splash();

            foreach (var module in _modules)
            {
                await module.RunAsync();
            }
        }

        private void SetupModules()
        {
            SimpleIoc.Default.Register<MainMod>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<MainMod>(), nameof(MainMod));

            SimpleIoc.Default.Register<AboutMod>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<AboutMod>(), nameof(AboutMod));

            SimpleIoc.Default.Register<HelpMod>();
            SimpleIoc.Default.Register<IModule>(
                () => SimpleIoc.Default.GetInstance<HelpMod>(), nameof(HelpMod));
        }

        private void SetupSubModules()
        {
            SimpleIoc.Default.Register<LoginSubMod>();
            SimpleIoc.Default.Register<ISubModule>(
                () => SimpleIoc.Default.GetInstance<LoginSubMod>(), nameof(LoginSubMod));

            SimpleIoc.Default.Register<IndexSubMod>();
            SimpleIoc.Default.Register<ISubModule>(
                () => SimpleIoc.Default.GetInstance<IndexSubMod>(), nameof(IndexSubMod));
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

            SimpleIoc.Default.Register<IDataService<Lookup>, LookupDataService>();
            SimpleIoc.Default.Register<IDataService<Requirement>, RequirementDataService>();

            if (!SimpleIoc.Default.IsRegistered<IHttpDataServiceFacade>())
            {
                if (Props.ApiBaseURL == Props.MOCKED)
                {
                    SimpleIoc.Default.Register<IHttpDataServiceFacade>(() =>
                    {
                        return new MockedHttpDataServiceFacade();
                    });
                }
                else
                {
                    SimpleIoc.Default.Register<IHttpDataServiceFacade>(() =>
                    {
                        return new HttpDataServiceFacade();
                    });
                }
            }
        }
    }
}
