using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.WPF.ViewModels;
using UrnaEletronicaFake.WPF.Services;

namespace UrnaEletronicaFake.WPF.Extensions;

public static class WPFServiceCollectionExtensions
{
    public static IServiceCollection AddWPFServices(this IServiceCollection services)
    {
        // Registrar serviços específicos do WPF
        services.AddSingleton<WindowManager>();
        
        // Registrar ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<DashboardWindowViewModel>();
        services.AddTransient<AdminWindowViewModel>();
        services.AddTransient<VotacaoWindowViewModel>();
        services.AddTransient<MesaWindowViewModel>();
        
        return services;
    }
}
