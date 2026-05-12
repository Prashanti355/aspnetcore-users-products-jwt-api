using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UsersProducts.Api.Common.Errors;
using UsersProducts.Api.Common.Exceptions;

namespace UsersProducts.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorResponseAsync(
                httpContext,
                StatusCodes.Status404NotFound,
                "Not Found",
                exception.Message
            );
        }
        catch (ConflictException exception)
        {
            await WriteErrorResponseAsync(
                httpContext,
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message
            );
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException &&
                  postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            await WriteErrorResponseAsync(
                httpContext,
                StatusCodes.Status409Conflict,
                "Conflict",
                "Ya existe un registro con un valor único duplicado."
            );
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrió un error inesperado.");

            await WriteErrorResponseAsync(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "Ocurrió un error interno en el servidor."
            );
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext httpContext,
        int statusCode,
        string error,
        string message)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        var response = new ApiErrorResponse(
            StatusCode: statusCode,
            Error: error,
            Message: message,
            Path: httpContext.Request.Path.Value ?? string.Empty,
            TimestampUtc: DateTime.UtcNow
        );

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions)
        );
    }
}