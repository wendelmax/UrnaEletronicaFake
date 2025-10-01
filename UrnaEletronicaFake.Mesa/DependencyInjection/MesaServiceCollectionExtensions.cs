using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Mesa.ViewModels;

namespace UrnaEletronicaFake.Modules.Mesa.DependencyInjection;

public static class MesaServiceCollectionExtensions
{
    public static IServiceCollection AddMesaModule(this IServiceCollection services)
    {
        services.AddTransient<MesaViewModel>();
        
        return services;
    }
}