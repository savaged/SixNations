using GalaSoft.MvvmLight;
using System;

namespace savaged.mvvm.Navigation
{
    public class NavigationService : ObservableObject, INavigationService
    {        
        public NavigationService(
            IDialogService dialogService, 
            IMainTabService mainTabService)
        {
            DialogService = dialogService;
            MainTabService = mainTabService;
        }

        public IMainTabService MainTabService { get; }

        public IDialogService DialogService { get; }

        public string CurrentPageKey => MainTabService.Selected.ToString();

        public void GoHome()
        {
            NavigateTo(MainTabService.Home.Key);
        }

        public void GoBack()
        {
            NavigateTo(MainTabService.Previous.Key);
        }

        public void NavigateTo(string viewKey)
        {
            NavigateTo(viewKey, null);
        }

        public void NavigateTo(string viewKey, object parameter)
        {
            if (DialogService.Contains(viewKey))
            {
                DialogService.Show(viewKey, parameter);
            }
            else if (MainTabService.Contains(viewKey))
            {
                MainTabService.Show(viewKey, parameter);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(viewKey),
                    "Navigation failed to locate view specified! " +
                    $"The viewKey supplied is: [{viewKey}]. Please" +
                    " make sure your view models implement the " +
                    $"{nameof(IFocusable)} or " +
                    $"{nameof(INavigableDialogViewModel)} interfaces " +
                    "in this library. And ensure you have " +
                    "added each view model to an implementation of " +
                    "CommonServiceLocator for IoC.");
            }
        }
    }
}
