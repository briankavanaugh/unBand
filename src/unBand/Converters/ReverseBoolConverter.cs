using System;
using System.Globalization;
using System.Windows.Data;

namespace unBand
{
    internal class ReverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as bool?;

            if (source == null)
            {
                return true;
            }

            return !(bool) source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}