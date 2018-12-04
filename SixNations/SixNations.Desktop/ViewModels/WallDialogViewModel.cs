using System;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmDialogs;
using SixNations.API.Interfaces;
using SixNations.Desktop.Messages;

namespace SixNations.Desktop.ViewModels
{
    public class WallDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        private readonly IKeepAuthAliveService _keepAuthAliveService;

        public WallDialogViewModel(IKeepAuthAliveService keepAuthAliveService)
        {
            _keepAuthAliveService = keepAuthAliveService;
            Title = Assembly.GetEntryAssembly().GetName().Name;
            FullScreenExitCmd = new RelayCommand(OnFullScreenExit, () => true);
            MessengerInstance.Register<CloseDialogRequestMessage>(this, OnCloseRequest);
        }

        public override void Cleanup()
        {
            _keepAuthAliveService.Kill();
            base.Cleanup();
        }

        public void Initialise()
        {
            _keepAuthAliveService.Start();
        }

        public bool? DialogResult { get; private set; }

        public string Title { get; }

        public ICommand FullScreenExitCmd { get; }

        private void OnFullScreenExit()
        {
            _keepAuthAliveService.Stop();
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
