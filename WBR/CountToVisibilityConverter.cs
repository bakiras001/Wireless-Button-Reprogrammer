using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WBR
{
    /// <summary>
    /// Returns Visible when the bound count is 0 (no devices detected yet),
    /// and Collapsed once one or more devices are present.
    /// Used to show a single empty placeholder row before any device connects.
    /// </summary>
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = 0;
            if (value is int i)
                count = i;

            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
