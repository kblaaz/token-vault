using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Aveva.Gdp.TokenVault.UI.Converters
{
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)new BoolToVisibilityConverter().Convert(value, targetType, parameter, culture);
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}