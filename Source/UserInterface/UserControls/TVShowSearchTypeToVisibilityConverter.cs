using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PerfectCode.TVShowManager.UserInterface.UserControls
{
    public class TVShowSearchTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = value.Equals(parameter);
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
