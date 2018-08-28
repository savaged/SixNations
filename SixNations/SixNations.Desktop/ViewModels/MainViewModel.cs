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
        private bool _isBusy;

        public MainViewModel(
            INavigationService navigationService, 
            MvvmDialogs.IDialogService dialogService)
        {
            _navigationService = navigationService;
            SelectedIndexManager = (ISelectedIndexManager)_navigationService;
            SelectedIndexManager.SelectedIndex = (int)HamburgerNavItemsIndex.Login;

            DialogService = dialogService;

            StoryFilterCmd = new RelayCommand(OnStoryFilter, CanExecuteStoryFilter);
            ClearStoryFilterCmd = new RelayCommand(OnClearStoryFilter, CanExecuteStoryFilter);
            ExitCmd = new RelayCommand(OnExit);
            
            MessengerInstance.Register<AuthenticatedMessage>(this, OnAuthenticated);
            MessengerInstance.Register<BusyMessage>(this, (m) => IsBusy = m.IsBusy);
        }

        public ISelectedIndexManager SelectedIndexManager { get; }

        public MvvmDialogs.IDialogService DialogService { get; }

        public ICommand ExitCmd { get; }

        public ICommand StoryFilterCmd { get; }

        public ICommand ClearStoryFilterCmd { get; }

        public bool CanExecuteStoryFilter => !IsBusy && IsLoggedIn;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsLoggedIn => User.Current.IsLoggedIn;

        private void OnAuthenticated(AuthenticatedMessage m)
        {
            IsBusy = true;
            User.Current.Initialise(m.Token);
            RaisePropertyChanged(nameof(IsLoggedIn));
            _navigationService.NavigateTo(
                HamburgerNavItemsIndex.Requirements.ToString());
            IsBusy = false;
        }

        private void OnStoryFilter()
        {
            var vm = ServiceLocator.Current.GetInstance<FindStoryDialogViewModel>();
            DialogService.ShowDialog(this, vm);
        }

        private void OnClearStoryFilter()
        {
            MessengerInstance.Send(new StoryFilterMessage(string.Empty));
        }

        private void OnExit()
        {
            App.Current.Shutdown();
        }
    }
}