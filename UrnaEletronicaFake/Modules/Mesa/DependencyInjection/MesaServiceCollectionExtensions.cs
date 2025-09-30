using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Modules.Mesa.ViewModels;
using UrnaEletronicaFake.Modules.Mesa.Services;
using UrnaEletronicaFake.Modules.Core.DependencyInjection;

namespace UrnaEletronicaFake.Modules.Mesa.DependencyInjection;

public static class MesaServiceCollectionExtensions
{
    public static IServiceCollection AddMesaModule(this IServiceCollection services)
    {
        services.AddCoreServices();
        
        services.AddSingleton<IMesaSecurityService, MesaSecurityService>();
        services.AddTransient<MesaViewModel>();
        
        return services;
    }
}