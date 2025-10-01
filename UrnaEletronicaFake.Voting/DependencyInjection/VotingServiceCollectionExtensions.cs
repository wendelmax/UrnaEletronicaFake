using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Voting.ViewModels;

namespace UrnaEletronicaFake.Modules.Voting.DependencyInjection;

public static class VotingServiceCollectionExtensions
{
    public static IServiceCollection AddVotingModule(this IServiceCollection services)
    {
        services.AddTransient<VotingViewModel>();
        
        return services;
    }
}