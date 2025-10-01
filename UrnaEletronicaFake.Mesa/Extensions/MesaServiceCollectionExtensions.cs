using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Mesa.Services;
using UrnaEletronicaFake.Mesa.Security;

namespace UrnaEletronicaFake.Mesa.Extensions;

public static class MesaServiceCollectionExtensions
{
    public static IServiceCollection AddMesaServices(this IServiceCollection services)
    {
        // Registrar serviços principais
        services.AddScoped<IMesaService, MesaService>();
        services.AddScoped<IMesaSecurityService, MesaSecurityService>();
        
        return services;
    }
}

