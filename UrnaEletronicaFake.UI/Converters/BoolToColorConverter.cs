using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace UrnaEletronicaFake.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? new SolidColorBrush(Color.Parse("#4CAF50")) : new SolidColorBrush(Color.Parse("#CF6679"));
        }
        return new SolidColorBrush(Color.Parse("#CF6679"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 