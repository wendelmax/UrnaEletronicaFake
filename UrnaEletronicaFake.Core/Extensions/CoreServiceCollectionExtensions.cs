using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Core.Interfaces;
using UrnaEletronicaFake.Core.Services;
using UrnaEletronicaFake.Core.EventHandlers;
using MediatR;

namespace UrnaEletronicaFake.Core.Extensions;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Registrar MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CoreServiceCollectionExtensions).Assembly));
        
        // Registrar serviços principais
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<ITerminalStateService, TerminalStateService>();
        
        // Registrar event handlers
        services.AddSingleton<CoreEventHandlers>();
        
        return services;
    }

    public static IServiceCollection AddCoreEventHandlers(this IServiceCollection services)
    {
        services.AddSingleton<CoreEventHandlers>();
        return services;
    }
}