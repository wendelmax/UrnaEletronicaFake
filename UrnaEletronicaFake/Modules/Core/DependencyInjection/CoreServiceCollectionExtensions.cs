using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.EventHandlers;

namespace UrnaEletronicaFake.Modules.Core.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<ITerminalStateService, TerminalStateService>();
        
        services.AddTransient<TerminalEventHandler>();
        
        return services;
    }
}