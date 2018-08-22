using System;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Windows.Storage;
using SixNations.Services;
using SixNations.Messages;
using Windows.UI.Xaml.Controls;

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

        private async void OnSubmit(object o)
        {
            if (!(o is PasswordBox))
            {
                throw new ArgumentException("Expected a PasswordBox!");
            }
            var pb = (PasswordBox)o;
            if (string.IsNullOrEmpty(pb.Password))
            {
                throw new ArgumentException("Expected to have a password set!");
            }
            var token = await new AuthTokenService().GetTokenAsync(Email, pb.Password);
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Expected a valid token!");
            }
            MessengerInstance.Send(new AuthenticatedMessage(token));
        }
    }
}
