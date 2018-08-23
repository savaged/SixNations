using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

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
                // Create design time view services and models
                //SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                // Create run time view services and models
                //SimpleIoc.Default.Register<IDataService, DataService>();
            }
            if (!SimpleIoc.Default.IsRegistered<MvvmDialogs.IDialogService>())
            {
                SimpleIoc.Default.Register<MvvmDialogs.IDialogService>(() => new MvvmDialogs.DialogService());
            }
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AboutDialogViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public AboutDialogViewModel About => ServiceLocator.Current.GetInstance<AboutDialogViewModel>();

        public LoginViewModel Login => ServiceLocator.Current.GetInstance<LoginViewModel>();

        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MvvmDialogs.IDialogService>();
        }
    }
}