using System;
using System.Globalization;
using System.Windows.Data;

namespace PMS_WPF_NET8.Converters
{
    public class BooleanToEditSaveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Edit" : "Save";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Edit";
        }
    }
}
