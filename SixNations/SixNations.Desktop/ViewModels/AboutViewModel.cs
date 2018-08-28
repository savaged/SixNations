using GalaSoft.MvvmLight;
using MvvmDialogs;
using System.Reflection;

namespace SixNations.Desktop.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            VersionText = $"Assure Desktop v{Assembly.GetEntryAssembly().GetName().Version}";
        }

        public string VersionText { get; }
    }
}
