using MvvmDialogs;
using System.Windows.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.StaticData;

namespace SixNations.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase, ISelectedIndexManager
    {
        private int _selectedNavIndex;

        public MainViewModel(IDialogService dialogService)
        {
            _selectedNavIndex = (int)NavItemsIndex.Login;

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

        public IDialogService DialogService { get; }

        public ICommand ExitCmd { get; }

        public ICommand ShowAboutDialogCmd { get; }

        public int SelectedIndex
        {
            get => _selectedNavIndex;
            set => Set(ref _selectedNavIndex, value);
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);

        public string AuthToken { get; private set; }

        private void OnAuthenticated(AuthenticatedMessage m)
        {
            AuthToken = m.Token;
            RaisePropertyChanged(() => IsLoggedIn);
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