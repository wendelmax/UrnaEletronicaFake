using System;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.EventHandlers;

namespace UrnaEletronicaFake.Modules.Core.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CoreServiceCollectionExtensions).Assembly);
        });

        services.AddScoped<IEventBus, EventBusService>();
        services.AddSingleton<ITerminalStateService, TerminalStateService>();

        services.AddScoped<TerminalStartedEventHandler>();
        services.AddScoped<TerminalStoppedEventHandler>();
        services.AddScoped<TerminalStateChangedEventHandler>();
        services.AddScoped<TerminalErrorEventHandler>();
        services.AddScoped<TerminalLogEventHandler>();

        return services;
    }
}