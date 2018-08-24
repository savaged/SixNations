using System;
using System.Windows.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Constants;
using GalaSoft.MvvmLight.Views;
using SixNations.Desktop.Models;

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

            MessengerInstance.Register<AuthenticatedMessage>(this, OnAuthenticated);
        }

        public ISelectedIndexManager SelectedIndexManager { get; }

        public MvvmDialogs.IDialogService DialogService { get; }

        public ICommand ExitCmd { get; }

        public ICommand ShowAboutDialogCmd { get; }

        public bool IsLoggedIn => User.Current.IsLoggedIn;

        private void OnAuthenticated(AuthenticatedMessage m)
        {
            User.Current.Initialise(m.Token);
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