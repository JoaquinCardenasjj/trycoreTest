using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrycoreEvm.Infrastructure.Persistence;

namespace TrycoreEvm.IntegrationTests;

/// <summary>
/// Reemplaza el proveedor PostgreSQL de <see cref="EvmDbContext"/> por una base de datos
/// en memoria, aislada por instancia, para que las pruebas de integración corran sin
/// depender de una base de datos real.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<EvmDbContext>>();

            services.AddDbContext<EvmDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
