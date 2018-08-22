using System;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.Storage;

namespace SixNations.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _email;
        private RelayCommand<object> _submitCmd;

        public LoginViewModel()
        {
            _email = (string)ApplicationData.Current.LocalSettings.Values["user_email"];

            _submitCmd = new RelayCommand<object>(OnSubmit, (p) => CanExecute);
        }

        public ICommand SubmitCmd
        {
            get => _submitCmd;
            set => _submitCmd = value != null ? (RelayCommand<object>)value : null;
        }

        public bool CanExecute
        {
            get
            {
                var canExecute = !string.IsNullOrEmpty(Email);
                return canExecute;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    Set(ref _email, value);
                    ApplicationData.Current.LocalSettings.Values["user_email"] = value;
                    _submitCmd.RaiseCanExecuteChanged();
                }
            }
        }

        private void OnSubmit(object password)
        {
            throw new NotImplementedException();
        }
    }
}
