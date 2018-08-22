using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SixNations.Desktop.Messages;
using System.Windows.Input;

namespace SixNations.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDialogService dialogService)
        {
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