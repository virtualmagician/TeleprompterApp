using System;
using System.Globalization;
using System.Windows.Data;

namespace TeleprompterApp
{
    public class VerticalCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double windowHeight = (double)values[0];
            double elementHeight = (double)values[1];
            return (windowHeight - elementHeight) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
