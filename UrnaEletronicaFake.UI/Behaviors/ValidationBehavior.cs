using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Data;
using System.Collections.Generic;
using System.Linq;

namespace UrnaEletronicaFake.UI.Behaviors;

public class ValidationBehavior : AvaloniaObject
{
    public static readonly StyledProperty<string?> ValidationErrorProperty =
        AvaloniaProperty.Register<ValidationBehavior, string?>(nameof(ValidationError));
    
    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<ValidationBehavior, bool>(nameof(IsRequired), defaultValue: false);
    
    public static readonly StyledProperty<int> MinLengthProperty =
        AvaloniaProperty.Register<ValidationBehavior, int>(nameof(MinLength), defaultValue: 0);
    
    public static readonly StyledProperty<int> MaxLengthProperty =
        AvaloniaProperty.Register<ValidationBehavior, int>(nameof(MaxLength), defaultValue: int.MaxValue);
    
    public static readonly StyledProperty<string?> PatternProperty =
        AvaloniaProperty.Register<ValidationBehavior, string?>(nameof(Pattern));
    
    public string? ValidationError
    {
        get => GetValue(ValidationErrorProperty);
        set => SetValue(ValidationErrorProperty, value);
    }
    
    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }
    
    public int MinLength
    {
        get => GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }
    
    public int MaxLength
    {
        get => GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }
    
    public string? Pattern
    {
        get => GetValue(PatternProperty);
        set => SetValue(PatternProperty, value);
    }
    
    public static void SetValidationError(AvaloniaObject element, string? value)
    {
        element.SetValue(ValidationErrorProperty, value);
    }
    
    public static string? GetValidationError(AvaloniaObject element)
    {
        return element.GetValue(ValidationErrorProperty);
    }
    
    public static void SetIsRequired(AvaloniaObject element, bool value)
    {
        element.SetValue(IsRequiredProperty, value);
    }
    
    public static bool GetIsRequired(AvaloniaObject element)
    {
        return element.GetValue(IsRequiredProperty);
    }
    
    public static void SetMinLength(AvaloniaObject element, int value)
    {
        element.SetValue(MinLengthProperty, value);
    }
    
    public static int GetMinLength(AvaloniaObject element)
    {
        return element.GetValue(MinLengthProperty);
    }
    
    public static void SetMaxLength(AvaloniaObject element, int value)
    {
        element.SetValue(MaxLengthProperty, value);
    }
    
    public static int GetMaxLength(AvaloniaObject element)
    {
        return element.GetValue(MaxLengthProperty);
    }
    
    public static void SetPattern(AvaloniaObject element, string? value)
    {
        element.SetValue(PatternProperty, value);
    }
    
    public static string? GetPattern(AvaloniaObject element)
    {
        return element.GetValue(PatternProperty);
    }
}

