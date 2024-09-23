﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace TeleprompterApp
{
    public class VerticalCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return 0.0;

            if (values[0] is double windowHeight && values[1] is double elementHeight)
            {
                return (windowHeight - elementHeight) / 2;
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
