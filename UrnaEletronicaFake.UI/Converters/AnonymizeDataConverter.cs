using System;
using System.Globalization;
using Avalonia.Data.Converters;
using UrnaEletronicaFake.UI.Services;

namespace UrnaEletronicaFake.UI.Converters;

public class AnonymizeDataConverter : IValueConverter
{
    private static readonly IDataAnonymizationService _anonymizationService = new DataAnonymizationService();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string data || string.IsNullOrEmpty(data))
            return value;
            
        var anonymizationType = parameter?.ToString()?.ToLowerInvariant();
        
        return anonymizationType switch
        {
            "cpf" => _anonymizationService.AnonymizeCpf(data),
            "email" => _anonymizationService.AnonymizeEmail(data),
            "phone" => _anonymizationService.AnonymizePhone(data),
            "name" => _anonymizationService.AnonymizeName(data),
            "address" => _anonymizationService.AnonymizeAddress(data),
            "document" => _anonymizationService.AnonymizeDocument(data),
            "auto" => _anonymizationService.IsSensitiveData(data) ? 
                     AutoAnonymize(data) : data,
            _ => data
        };
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    
    private static string AutoAnonymize(string data)
    {
        if (data.Contains("@"))
            return _anonymizationService.AnonymizeEmail(data);
        if (data.Contains("-") && data.Length == 11)
            return _anonymizationService.AnonymizeCpf(data);
        if (data.Contains("(") || data.Contains(")"))
            return _anonymizationService.AnonymizePhone(data);
        if (data.Contains("/") && data.Length > 10)
            return _anonymizationService.AnonymizeDocument(data);
            
        return _anonymizationService.AnonymizeName(data);
    }
}


