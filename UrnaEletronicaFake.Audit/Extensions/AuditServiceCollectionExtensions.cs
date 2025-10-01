using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Audit.Services;
using UrnaEletronicaFake.Audit.Reports;
using UrnaEletronicaFake.Audit.Storage;

namespace UrnaEletronicaFake.Audit.Extensions;

public static class AuditServiceCollectionExtensions
{
    public static IServiceCollection AddAuditServices(this IServiceCollection services)
    {
        // Registrar serviços principais
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuditReportService, AuditReportService>();
        services.AddScoped<IAuditStorageService, AuditStorageService>();
        
        return services;
    }
}

