using GalaSoft.MvvmLight;
using MahApps.Metro;
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
                    var currentTheme = ThemeManager.DetectAppStyle(App.Current);
                    ThemeManager.ChangeAppStyle(
                        App.Current, 
                        currentTheme.Item2, 
                        ThemeManager.GetAppTheme($"Base{value.ToString()}")
                    );
                    Properties.Settings.Default.ThemeOption = value;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged();
                }
            }
        }
    }
}
