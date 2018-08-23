using System;
using System.Windows.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Constants;
using GalaSoft.MvvmLight.Views;

namespace SixNations.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase, IShellViewModel
    {
        private readonly INavigationService _navigationService;

        public MainViewModel(
            INavigationService navigationService, 
            MvvmDialogs.IDialogService dialogService)
        {
            _navigationService = navigationService;
            SelectedIndexManager = (ISelectedIndexManager)_navigationService;
            SelectedIndexManager.SelectedIndex = (int)HamburgerNavItemsIndex.Login;

            DialogService = dialogService;

            ExitCmd = new RelayCommand(OnExit);
            ShowAboutDialogCmd = new RelayCommand(OnShowAboutDialog);

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
            MessengerInstance.Register<AuthenticatedMessage>(this, OnAuthenticated);
        }

        public ISelectedIndexManager SelectedIndexManager { get; }

        public MvvmDialogs.IDialogService DialogService { get; }

        public ICommand ExitCmd { get; }

        public ICommand ShowAboutDialogCmd { get; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);

        public string AuthToken { get; private set; }

        private void OnAuthenticated(AuthenticatedMessage m)
        {
            AuthToken = m.Token;
            RaisePropertyChanged(() => IsLoggedIn);
            _navigationService.NavigateTo(
                HamburgerNavItemsIndex.Requirements.ToString());
        }

        private void OnShowAboutDialog()
        {
            var vm = ServiceLocator.Current.GetInstance<AboutDialogViewModel>();
            DialogService.ShowDialog(this, vm);
        }

        private void OnExit()
        {
            App.Current.Shutdown();
        }
    }
}