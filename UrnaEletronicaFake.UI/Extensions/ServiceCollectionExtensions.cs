using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using UrnaEletronicaFake.Core.Extensions;
using UrnaEletronicaFake.Data.Extensions;
using UrnaEletronicaFake.Voting.Extensions;
using UrnaEletronicaFake.Mesa.Extensions;
using UrnaEletronicaFake.Audit.Extensions;
using UrnaEletronicaFake.Administration.Extensions;
using UrnaEletronicaFake.Dashboard.Extensions;
using UrnaEletronicaFake.UI.Extensions;
using UrnaEletronicaFake.Core.EventHandlers;
using UrnaEletronicaFake.Voting.Events;
using FluentValidation; // add for AddValidatorsFromAssembly

namespace UrnaEletronicaFake.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrnaEletronicaFakeServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configurar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // Configurar banco de dados
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=urna_eletronica.db";
        services.AddDataServices(connectionString);

        // Configurar módulos
        services.AddCoreServices();
        services.AddVotingServices();
        services.AddMesaServices();
        services.AddAuditServices();
        services.AddAdministrationServices();
        services.AddDashboardServices();
        services.AddUIServices();

        // Configurar event handlers
        services.AddCoreEventHandlers();
        
        // Configurar validação
        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        return services;
    }

    public static IServiceCollection AddUrnaEletronicaFakeServices(
        this IServiceCollection services, 
        string connectionString)
    {
        // Configurar logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        // Configurar banco de dados
        services.AddDataServices(connectionString);

        // Configurar módulos
        services.AddCoreServices();
        services.AddVotingServices();
        services.AddMesaServices();
        services.AddAuditServices();
        services.AddAdministrationServices();
        services.AddDashboardServices();
        services.AddUIServices();

        // Configurar event handlers
        services.AddCoreEventHandlers();
        
        // Configurar validação
        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        return services;
    }
}
