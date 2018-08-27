using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService<Requirement>, DesignRequirementDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService<Requirement>, RequirementDataService>();
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
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AboutDialogViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RequirementViewModel>();
            SimpleIoc.Default.Register<WallViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public AboutDialogViewModel About => ServiceLocator.Current.GetInstance<AboutDialogViewModel>();

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public RequirementViewModel Requirement => ServiceLocator.Current.GetInstance<RequirementViewModel>();

        public WallViewModel Wall => ServiceLocator.Current.GetInstance<WallViewModel>();

        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MvvmDialogs.IDialogService>();
        }
    }
}