using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RealEstate.App.Controls
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.OrangeRed;

            return (bool)value ? Brushes.LawnGreen : Brushes.OrangeRed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return (SolidColorBrush)value == Brushes.LawnGreen;
        }
    }
}