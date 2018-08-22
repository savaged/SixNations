using GalaSoft.MvvmLight;
using MvvmDialogs;
using System.Reflection;

namespace SixNations.Desktop.ViewModels
{
    public class AboutDialogViewModel : ViewModelBase, IModalDialogViewModel
    {
        public AboutDialogViewModel()
        {
            VersionText = $"Assure Desktop v{Assembly.GetEntryAssembly().GetName().Version}";
        }

        public string VersionText { get; }

        public bool? DialogResult => true;
    }
}
