using Microsoft.Extensions.DependencyInjection;
using TrycoreEvm.Application.Services;

namespace TrycoreEvm.Application;

/// <summary>Registra los servicios de la capa de aplicación en el contenedor de DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IActivityService, ActivityService>();

        return services;
    }
}
