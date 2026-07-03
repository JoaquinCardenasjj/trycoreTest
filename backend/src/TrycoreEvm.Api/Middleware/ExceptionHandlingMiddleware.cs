using System.Net;
using Microsoft.AspNetCore.Mvc;
using TrycoreEvm.Domain.Exceptions;

namespace TrycoreEvm.Api.Middleware;

/// <summary>
/// Traduce las excepciones del dominio y la aplicación a respuestas HTTP con formato
/// ProblemDetails, para que ningún controlador necesite bloques try/catch propios.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException exception)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, exception.Message);
        }
        catch (DomainValidationException exception)
        {
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error no controlado procesando la solicitud {Path}", context.Request.Path);
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado procesando la solicitud.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = statusCode.ToString(),
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
