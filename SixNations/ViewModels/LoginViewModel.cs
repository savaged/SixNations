using System;
using GalaSoft.MvvmLight;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace SixNations.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _email;

        public LoginViewModel()
        {
            _email = string.Empty;
            SubmitCmd = new RelayCommand<string>(OnSubmit, (b) => CanSubmit);
        }

        public ICommand SubmitCmd { get; set; }

        public bool CanSubmit => !string.IsNullOrEmpty(Email);

        public string Email
        {
            get => _email;
            set
            {
                Set(ref _email, value);
                RaisePropertyChanged(nameof(CanSubmit));
            }
        }

        private void OnSubmit(string password)
        {
            throw new NotImplementedException();
        }
    }
}
