using System;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SixNations.Desktop.Messages;

namespace SixNations.Desktop.ViewModels
{
    public class WallDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        public WallDialogViewModel()
        {
            Title = Assembly.GetEntryAssembly().GetName().Name;
            FullScreenExitCmd = new RelayCommand(OnFullScreenExit, () => true);
            MessengerInstance.Register<CloseDialogRequestMessage>(this, OnCloseRequest);
        }

        public bool? DialogResult { get; private set; }

        public string Title { get; }

        public ICommand FullScreenExitCmd { get; }

        private void OnFullScreenExit()
        {
            DialogResult = true;
            RaisePropertyChanged(nameof(DialogResult));
        }

        private void OnCloseRequest(CloseDialogRequestMessage m)
        {
            if (m.Sender is PostItViewModel)
            {
                DialogResult = m.DialogResult;
                RaisePropertyChanged(nameof(DialogResult));
            }
        }
    }
}
