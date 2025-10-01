using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace UrnaEletronicaFake.Converters;

public class BoolToStatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "ATIVA" : "INATIVA";
        }
        return "INATIVA";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 