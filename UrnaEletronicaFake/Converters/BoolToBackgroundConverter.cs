using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace UrnaEletronicaFake.Converters;

public class BoolToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? new SolidColorBrush(Color.Parse("#dcfce7")) : new SolidColorBrush(Color.Parse("#fef2f2"));
        }
        return new SolidColorBrush(Color.Parse("#fef2f2"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 