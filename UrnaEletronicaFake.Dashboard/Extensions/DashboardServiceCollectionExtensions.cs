using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Dashboard.Services;
using UrnaEletronicaFake.Dashboard.Reports;

namespace UrnaEletronicaFake.Dashboard.Extensions;

public static class DashboardServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardServices(this IServiceCollection services)
    {
        // Registrar serviços principais
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDashboardReportService, DashboardReportService>();
        
        return services;
    }
}


