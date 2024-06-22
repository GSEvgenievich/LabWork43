using System.Globalization;
using System.Windows.Data;

namespace Task3._1
{
    public class HalfWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            return (width / 2) - 50; // Предполагая, что ширина ракетки 100
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
