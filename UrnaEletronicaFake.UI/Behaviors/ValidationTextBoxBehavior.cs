using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;

namespace UrnaEletronicaFake.UI.Behaviors;

public class ValidationTextBoxBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<string?> ValidationErrorProperty =
        AvaloniaProperty.RegisterAttached<ValidationTextBoxBehavior, TextBox, string?>(
            "ValidationError", inherits: true);
    
    public static readonly AttachedProperty<bool> IsRequiredProperty =
        AvaloniaProperty.RegisterAttached<ValidationTextBoxBehavior, TextBox, bool>(
            "IsRequired", defaultValue: false);
    
    public static readonly AttachedProperty<int> MinLengthProperty =
        AvaloniaProperty.RegisterAttached<ValidationTextBoxBehavior, TextBox, int>(
            "MinLength", defaultValue: 0);
    
    public static readonly AttachedProperty<int> MaxLengthProperty =
        AvaloniaProperty.RegisterAttached<ValidationTextBoxBehavior, TextBox, int>(
            "MaxLength", defaultValue: int.MaxValue);
    
    public static readonly AttachedProperty<string?> PatternProperty =
        AvaloniaProperty.RegisterAttached<ValidationTextBoxBehavior, TextBox, string?>(
            "Pattern");
    
    public static string? GetValidationError(TextBox element)
    {
        return element.GetValue(ValidationErrorProperty);
    }
    
    public static void SetValidationError(TextBox element, string? value)
    {
        element.SetValue(ValidationErrorProperty, value);
    }
    
    public static bool GetIsRequired(TextBox element)
    {
        return element.GetValue(IsRequiredProperty);
    }
    
    public static void SetIsRequired(TextBox element, bool value)
    {
        element.SetValue(IsRequiredProperty, value);
        if (value)
        {
            element.LostFocus += OnLostFocus;
            element.TextChanged += OnTextChanged;
        }
        else
        {
            element.LostFocus -= OnLostFocus;
            element.TextChanged -= OnTextChanged;
        }
    }
    
    public static int GetMinLength(TextBox element)
    {
        return element.GetValue(MinLengthProperty);
    }
    
    public static void SetMinLength(TextBox element, int value)
    {
        element.SetValue(MinLengthProperty, value);
    }
    
    public static int GetMaxLength(TextBox element)
    {
        return element.GetValue(MaxLengthProperty);
    }
    
    public static void SetMaxLength(TextBox element, int value)
    {
        element.SetValue(MaxLengthProperty, value);
    }
    
    public static string? GetPattern(TextBox element)
    {
        return element.GetValue(PatternProperty);
    }
    
    public static void SetPattern(TextBox element, string? value)
    {
        element.SetValue(PatternProperty, value);
    }
    
    private static void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            ValidateTextBox(textBox);
        }
    }
    
    private static void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            ValidateTextBox(textBox);
        }
    }
    
    private static void ValidateTextBox(TextBox textBox)
    {
        var text = textBox.Text ?? string.Empty;
        var isRequired = GetIsRequired(textBox);
        var minLength = GetMinLength(textBox);
        var maxLength = GetMaxLength(textBox);
        var pattern = GetPattern(textBox);
        
        string? error = null;
        
        if (isRequired && string.IsNullOrWhiteSpace(text))
        {
            error = "Este campo é obrigatório";
        }
        else if (!string.IsNullOrWhiteSpace(text))
        {
            if (text.Length < minLength)
            {
                error = $"Mínimo de {minLength} caracteres";
            }
            else if (text.Length > maxLength)
            {
                error = $"Máximo de {maxLength} caracteres";
            }
            else if (!string.IsNullOrEmpty(pattern) && !Regex.IsMatch(text, pattern))
            {
                error = "Formato inválido";
            }
        }
        
        SetValidationError(textBox, error);
        UpdateVisualState(textBox, error);
    }
    
    private static void UpdateVisualState(TextBox textBox, string? error)
    {
        if (string.IsNullOrEmpty(error))
        {
            textBox.BorderBrush = Avalonia.Media.Brushes.Green;
            textBox.BorderThickness = new Avalonia.Thickness(2);
        }
        else
        {
            textBox.BorderBrush = Avalonia.Media.Brushes.Red;
            textBox.BorderThickness = new Avalonia.Thickness(2);
        }
    }
}

