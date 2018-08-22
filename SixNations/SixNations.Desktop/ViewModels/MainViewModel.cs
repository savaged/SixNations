using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using System.Windows.Input;

namespace SixNations.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDialogService dialogService)
        {
            DialogService = dialogService;

            ShowAboutDialogCmd = new RelayCommand(OnShowAboutDialog);

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }

        public IDialogService DialogService { get; }

        public ICommand ShowAboutDialogCmd { get; }

        private void OnShowAboutDialog()
        {
            var vm = ServiceLocator.Current.GetInstance<AboutDialogViewModel>();
            DialogService.ShowDialog(this, vm);
        }
    }
}