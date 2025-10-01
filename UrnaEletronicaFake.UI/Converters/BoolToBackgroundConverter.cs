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
            return isActive ? new SolidColorBrush(Color.Parse("#E8F5E8")) : new SolidColorBrush(Color.Parse("#FFEBEE"));
        }
        return new SolidColorBrush(Color.Parse("#FFEBEE"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 