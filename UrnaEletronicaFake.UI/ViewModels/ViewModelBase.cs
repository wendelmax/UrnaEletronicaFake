using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace UrnaEletronicaFake.UI.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected readonly ILogger Logger;

    protected ViewModelBase(ILogger logger)
    {
        Logger = logger;
    }

    protected virtual void LogError(Exception ex, string message, params object[] args)
    {
        Logger.LogError(ex, message, args);
    }

    protected virtual void LogInformation(string message, params object[] args)
    {
        Logger.LogInformation(message, args);
    }

    protected virtual void LogWarning(string message, params object[] args)
    {
        Logger.LogWarning(message, args);
    }

    protected virtual void LogDebug(string message, params object[] args)
    {
        Logger.LogDebug(message, args);
    }
}