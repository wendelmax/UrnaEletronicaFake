using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Modules.Core.Services;
using UrnaEletronicaFake.Modules.Core.EventHandlers;

namespace UrnaEletronicaFake.Modules.Core.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CoreServiceCollectionExtensions).Assembly);
        });

        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<ITerminalStateService, TerminalStateService>();

        services.AddTransient<TerminalUnlockedEventHandler>();
        services.AddTransient<TerminalLockedEventHandler>();
        services.AddTransient<VoteStartedEventHandler>();
        services.AddTransient<VoteCompletedEventHandler>();
        services.AddTransient<VoteAbortedEventHandler>();
        services.AddTransient<TerminalErrorEventHandler>();

        return services;
    }
}