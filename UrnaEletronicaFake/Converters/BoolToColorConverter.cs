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
            return isActive ? new SolidColorBrush(Color.Parse("#166534")) : new SolidColorBrush(Color.Parse("#dc2626"));
        }
        return new SolidColorBrush(Color.Parse("#dc2626"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 