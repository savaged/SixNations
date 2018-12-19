using System.Deployment.Application;
using GalaSoft.MvvmLight;
using SixNations.Data.Models;

namespace SixNations.Desktop.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            string ver = null;
            try
            {
                ver = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            finally
            {
                SelectedItem = new About(ver);
            }
        }

        public About SelectedItem { get; }
    }
}
