using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Administration.Services;
using UrnaEletronicaFake.Administration.Managers;
using UrnaEletronicaFake.Administration.Validation;
using FluentValidation;
using UrnaEletronicaFake.Shared.Models;

namespace UrnaEletronicaFake.Administration.Extensions;

public static class AdministrationServiceCollectionExtensions
{
    public static IServiceCollection AddAdministrationServices(this IServiceCollection services)
    {
        // Registrar serviços principais
        services.AddScoped<IAdministrationService, AdministrationService>();
        services.AddScoped<IElectionManager, ElectionManager>();
        
        // Registrar validators
        services.AddScoped<IValidator<Eleicao>, EleicaoValidator>();
        services.AddScoped<IValidator<Candidato>, CandidatoValidator>();
        services.AddScoped<IValidator<CargoEleitoral>, CargoEleitoralValidator>();
        
        return services;
    }
}
