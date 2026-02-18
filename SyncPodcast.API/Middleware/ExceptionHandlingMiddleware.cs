using FluentValidation;
using SyncPodcast.Domain.Exceptions;
using System.Text.Json;

namespace SyncPodcast.API.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException validationException => (StatusCodes.Status400BadRequest, JsonSerializer.Serialize(validationException.Errors)),
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, notFoundException.Message),
            DomainException domainException => (StatusCodes.Status400BadRequest, domainException.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(json);
    }
}


