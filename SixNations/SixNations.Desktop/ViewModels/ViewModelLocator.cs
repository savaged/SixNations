// Pre Standard .Net (see http://www.mvvmlight.net/std10) using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SixNations.API.Constants;
using SixNations.Data.Facade;
using Savaged.BusyStateManager;
using SixNations.Desktop.Interfaces;
using SixNations.API.Interfaces;
using SixNations.Data.Models;
using SixNations.Data.Services;
using SixNations.Desktop.Services;

namespace SixNations.Desktop.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            // Pre Standard .Net (see http://www.mvvmlight.net/std10) ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

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
            if (!SimpleIoc.Default.IsRegistered<INavigationService>())
            {
                SimpleIoc.Default.Register<INavigationService>(() =>
                {
                    return new NavigationService();
                });
            }
            if (!SimpleIoc.Default.IsRegistered<MvvmDialogs.IDialogService>())
            {
                SimpleIoc.Default.Register<MvvmDialogs.IDialogService>(() =>
                {
                    return new MvvmDialogs.DialogService();
                });
            }
            SimpleIoc.Default.Register<IActionConfirmationService, ActionConfirmationService>();

            SimpleIoc.Default.Register<IBusyStateRegistry>(() =>
            {
                return new BusyStateRegistry();
            });

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RequirementViewModel>();
            SimpleIoc.Default.Register<WallViewModel>();
            SimpleIoc.Default.Register<WallDialogViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();            
            SimpleIoc.Default.Register<FindStoryDialogViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        public LoginViewModel Login => SimpleIoc.Default.GetInstance<LoginViewModel>();

        public RequirementViewModel Requirement => SimpleIoc.Default.GetInstance<RequirementViewModel>();

        public WallViewModel Wall => SimpleIoc.Default.GetInstance<WallViewModel>();

        public WallDialogViewModel WallDialog => SimpleIoc.Default.GetInstance<WallDialogViewModel>();

        public AboutViewModel About => SimpleIoc.Default.GetInstance<AboutViewModel>();

        public SettingsViewModel Settings => SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public FindStoryDialogViewModel FindStoryDialog => 
            SimpleIoc.Default.GetInstance<FindStoryDialogViewModel>();

        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MvvmDialogs.IDialogService>();
        }
    }
}