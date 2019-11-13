using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class HelpDialogViewModel 
        : ViewModelBase, 
        IObservableObject, 
        IHelpDialogViewModel,
        INavigableDialogViewModel
    {
        private string _source;

        public HelpDialogViewModel(string page)
        {
            HasFocus = true;
            _source = GlobalConstants.HELP_PAGES_LOCAION + page;
            ExitCmd = new RelayCommand(RaiseRequestClose);
        }

        public ICommand ExitCmd { get; } 

        public bool HasFocus { get; set; }

        public string Source
        {
            get => _source;
            private set => Set(ref _source, value);
        }

        public bool? DialogResult
        {
            get => true;
            set => value = true;
        }

        public event Action RequestClose = delegate { };

        public void RaiseRequestClose()
        {
            RequestClose?.Invoke();
        }

    }
}
