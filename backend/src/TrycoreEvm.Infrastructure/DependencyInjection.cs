using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrycoreEvm.Application.Interfaces;
using TrycoreEvm.Infrastructure.Persistence;
using TrycoreEvm.Infrastructure.Repositories;

namespace TrycoreEvm.Infrastructure;

/// <summary>Registra el DbContext y las implementaciones de persistencia en el contenedor de DI.</summary>
public static class DependencyInjection
{
    private const string ConnectionStringName = "EvmDatabase";

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"No se encontró la cadena de conexión '{ConnectionStringName}'.");

        services.AddDbContext<EvmDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectActivityRepository, ActivityProjectRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
