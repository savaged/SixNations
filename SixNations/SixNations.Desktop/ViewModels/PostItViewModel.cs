// Pre Standard .Net (see http://www.mvvmlight.net/std10) using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;
using SixNations.Desktop.Models;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Constants;
using SixNations.Desktop.Messages;

namespace SixNations.Desktop.ViewModels
{
    public class PostItViewModel : ViewModelBase
    {
        public PostItViewModel(Requirement requirement)
        {
            Requirement = requirement;
            NavigateToCmd = new RelayCommand(OnNavigateTo, () => true);
        }

        public ICommand NavigateToCmd { get; }

        private void OnNavigateTo()
        {
            var nav = SimpleIoc.Default.GetInstance<INavigationService>();
            nav.NavigateTo(HamburgerNavItemsIndex.Requirement.ToString(), Requirement);
            MessengerInstance.Send(new CloseDialogRequestMessage(this, true));
        }

        public Requirement Requirement { get; }
    }
}
