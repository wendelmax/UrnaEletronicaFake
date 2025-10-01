using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Voting.Services;
using UrnaEletronicaFake.Voting.Events;
using UrnaEletronicaFake.Voting.Validators;
using FluentValidation;
using UrnaEletronicaFake.Shared.DTOs;

namespace UrnaEletronicaFake.Voting.Extensions;

public static class VotingServiceCollectionExtensions
{
    public static IServiceCollection AddVotingServices(this IServiceCollection services)
    {
        // Registrar serviços principais
        services.AddScoped<IVotingService, VotingService>();
        
        // Registrar event handlers
        services.AddScoped<VotingEventHandlers>();
        
        // Registrar validators
        services.AddScoped<IValidator<VotingRequest>, VotingRequestValidator>();
        services.AddScoped<IValidator<VotingResult>, VotingResultValidator>();
        
        return services;
    }
}
