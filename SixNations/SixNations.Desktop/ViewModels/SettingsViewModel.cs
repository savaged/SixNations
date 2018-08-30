using GalaSoft.MvvmLight;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public ThemeOptions ThemeOption
        {
            get => Properties.Settings.Default.ThemeOption;
            set
            {
                if (Properties.Settings.Default.ThemeOption != value)
                {
                    Properties.Settings.Default.ThemeOption = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged();
                }
            }
        }
    }
}
