using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.UI.Services;

namespace UrnaEletronicaFake.UI.Extensions;

public static class UIServiceCollectionExtensions
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddScoped<IWindowManagerService, WindowManagerService>();
        
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.MainWindowViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.DashboardViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.MesaViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.VotacaoViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.AdminViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.AuditoriaViewModel>();
        services.AddScoped<UrnaEletronicaFake.UI.ViewModels.ResultadosViewModel>();
        
        return services;
    }
}
