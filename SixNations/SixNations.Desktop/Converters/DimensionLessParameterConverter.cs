using System;
using System.Globalization;
using System.Windows.Data;
using SixNations.API.Helpers;

namespace SixNations.Desktop.Converters
{
    public class DimensionLessParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.IsNumeric())
            {
                if (value != null && value.IsNumeric() &&
                    parameter != null && parameter is string s && s.IsNumeric())
                {
                    value = (double)value - int.Parse(s);
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
