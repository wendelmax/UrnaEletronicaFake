using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.WPF.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    protected readonly ILogger _logger;

    protected ViewModelBase(ILogger logger)
    {
        _logger = logger;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    protected void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message, args);
    }

    protected void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }
}
