using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace SixNations.Desktop.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                var tmp = (Nullable<bool>)value;
                bValue = tmp ?? false;
            }
            return (bValue) ? Visibility.Collapsed : Visibility.Visible; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Collapsed;
            }
            else
            {
                return true;
            }
        }
    }
}
