using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RealEstate.App.Library
{
    public class ColorMixer : IValueConverter
    {
        public double Amount { get; set; }

        private static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            var finalColor = Color.FromArgb(color.A, System.Convert.ToByte(red), System.Convert.ToByte(green), System.Convert.ToByte(blue));
            return finalColor;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush color)
            {
                var changedColor = ChangeColorBrightness(color.Color, Amount);
                var result = new SolidColorBrush(changedColor);
                return result;
            }

            if (!(value is LinearGradientBrush linearColor))
                return Brushes.White;

            if (!linearColor.GradientStops.Any())
                return Brushes.White;

            var changedColors = new LinearGradientBrush();
            foreach (var stop in linearColor.GradientStops)
            {
                var changedColor = ChangeColorBrightness(stop.Color, Amount);
                var gs = new GradientStop(changedColor, stop.Offset);
                changedColors.GradientStops.Add(gs);
            }

            changedColors.StartPoint = linearColor.StartPoint;
            changedColors.EndPoint = linearColor.EndPoint;
            return changedColors;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}