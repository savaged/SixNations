using MvvmDialogs;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SixNations.Desktop.Messages;
using System;
using System.Windows.Controls;

namespace SixNations.Desktop.ViewModels
{
    public class FindStoryDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        public FindStoryDialogViewModel()
        {
            SubmitCmd = new RelayCommand<object>(OnSubmit);
        }

        public bool? DialogResult { get; private set; }

        public ICommand SubmitCmd { get; }

        private void OnSubmit(object filterBox)
        {
            if (filterBox is null)
            {
                throw new ArgumentNullException("Expected a TextBox!");
            }
            var filter = ((TextBox)filterBox).Text;
            MessengerInstance.Send(new StoryFilterMessage(filter));
            DialogResult = true;
            RaisePropertyChanged(nameof(DialogResult));
        }
    }
}
