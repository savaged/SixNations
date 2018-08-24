using System;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using SixNations.Desktop.Services;
using SixNations.Desktop.Messages;
using System.Windows.Controls;
using SixNations.Desktop.Exceptions;
using SixNations.Desktop.Helpers;

namespace SixNations.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _email;
        private RelayCommand<object> _submitCmd;

        public LoginViewModel()
        {
            _email = Properties.Settings.Default.UserEmail;

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

                    Properties.Settings.Default.UserEmail = value;
                    Properties.Settings.Default.Save();

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
            string token = null;
            try
            {
                token = await new AuthTokenService().GetTokenAsync(Email, pb.Password);
            }
            catch (AuthServiceException ex)
            {
                FeedbackActions.ReactToException(ex);
            }
            if (!string.IsNullOrEmpty(token))
            {
                MessengerInstance.Send(new AuthenticatedMessage(token));
            }
        }
    }
}
