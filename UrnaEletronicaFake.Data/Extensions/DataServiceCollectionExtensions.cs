using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data.DbContext;
using UrnaEletronicaFake.Data.Repositories;

namespace UrnaEletronicaFake.Data.Extensions;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        // Configurar DbContext
        services.AddDbContext<UrnaDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.EnableSensitiveDataLogging(false);
            options.EnableServiceProviderCaching();
        });

        // Registrar repositórios
        services.AddScoped<IRepository<UrnaEletronicaFake.Shared.Models.Eleicao>, Repository<UrnaEletronicaFake.Shared.Models.Eleicao>>();
        services.AddScoped<IRepository<UrnaEletronicaFake.Shared.Models.CargoEleitoral>, Repository<UrnaEletronicaFake.Shared.Models.CargoEleitoral>>();
        services.AddScoped<ICandidatoRepository, CandidatoRepository>();
        services.AddScoped<IVotoRepository, VotoRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

        return services;
    }

    public static IServiceCollection AddDataServices(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
    {
        // Configurar DbContext com opções customizadas
        services.AddDbContext<UrnaDbContext>(optionsAction);

        // Registrar repositórios
        services.AddScoped<IRepository<UrnaEletronicaFake.Shared.Models.Eleicao>, Repository<UrnaEletronicaFake.Shared.Models.Eleicao>>();
        services.AddScoped<IRepository<UrnaEletronicaFake.Shared.Models.CargoEleitoral>, Repository<UrnaEletronicaFake.Shared.Models.CargoEleitoral>>();
        services.AddScoped<ICandidatoRepository, CandidatoRepository>();
        services.AddScoped<IVotoRepository, VotoRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

        return services;
    }
}

