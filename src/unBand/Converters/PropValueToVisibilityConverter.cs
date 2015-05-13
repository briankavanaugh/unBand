using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace unBand
{
    internal class PropValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // covers the null case
            if (value == null)
            {
                return (parameter == null ? Visibility.Visible : Visibility.Collapsed);
            }
            if (parameter == null)
            {
                return Visibility.Collapsed;
            }

            var strParameter = parameter as string;
            var strValue = value.ToString();

            if (strParameter.Contains("|"))
            {
                var parameters = strParameter.Split('|');

                foreach (var singleParam in parameters)
                {
                    if (strValue == singleParam)
                        return Visibility.Visible;
                }
            }

            return (strValue == strParameter ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}