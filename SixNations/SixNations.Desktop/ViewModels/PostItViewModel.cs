using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.ViewModels
{
    public class PostItViewModel : ViewModelBase
    {
        public PostItViewModel(Requirement requirement)
        {
            Requirement = requirement;
            GoToEditCmd = new RelayCommand<int>(OnGoToEdit, (b) => true);
        }

        public ICommand GoToEditCmd { get; }

        private void OnGoToEdit(int requirementId)
        {
            // TODO navigate to requirements screen and open editing
            throw new NotImplementedException();
        }

        public Requirement Requirement { get; }
    }
}
