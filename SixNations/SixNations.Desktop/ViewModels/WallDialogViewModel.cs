using System;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;

namespace SixNations.Desktop.ViewModels
{
    public class WallDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        public WallDialogViewModel()
        {
            Title = Assembly.GetEntryAssembly().GetName().Name;
            FullScreenExitCmd = new RelayCommand(OnFullScreenExit, () => true);
        }

        public bool? DialogResult { get; private set; }

        public string Title { get; }

        public ICommand FullScreenExitCmd { get; }

        private void OnFullScreenExit()
        {
            DialogResult = true;
            RaisePropertyChanged(nameof(DialogResult));
        }
    }
}
