using Microsoft.OpenApi.Models;
using TrycoreEvm.Api.Middleware;
using TrycoreEvm.Application;
using TrycoreEvm.Infrastructure;

const string AngularDevCorsPolicy = "AngularDevCorsPolicy";
var angularDevOrigins = new[] { "http://localhost:4200" };

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Trycore EVM API",
        Version = "v1",
        Description = "API para el seguimiento de proyectos mediante indicadores de Valor Ganado (EVM)."
    });

    var xmlDocumentationFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlDocumentationPath = Path.Combine(AppContext.BaseDirectory, xmlDocumentationFile);
    if (File.Exists(xmlDocumentationPath))
    {
        options.IncludeXmlComments(xmlDocumentationPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularDevCorsPolicy, policy =>
    {
        policy.WithOrigins(angularDevOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger(options => options.RouteTemplate = "api-docs/{documentName}/swagger.json");
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api-docs/v1/swagger.json", "Trycore EVM API v1");
    options.RoutePrefix = "api-docs";
});

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseCors(AngularDevCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Clase parcial pública requerida para que WebApplicationFactory pueda referenciar
/// el punto de entrada del API desde el proyecto de pruebas de integración.
/// </summary>
public partial class Program
{
}
