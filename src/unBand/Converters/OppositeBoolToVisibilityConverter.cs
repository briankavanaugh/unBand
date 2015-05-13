using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace unBand
{
    internal class OppositeBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as bool?;

            if (source == null)
            {
                return Visibility.Visible;
            }

            return (!(bool) source ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}