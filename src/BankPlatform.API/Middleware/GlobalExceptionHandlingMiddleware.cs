using System.Net;
using System.Text.Json;

namespace BankPlatform.API.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses
/// BoG compliance: Secure error handling without sensitive data exposure
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred. RequestId: {RequestId}", 
                context.TraceIdentifier);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid request parameters"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access denied"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Operation not allowed"),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout"),
            _ => (HttpStatusCode.InternalServerError, "An error occurred processing your request")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Error = new ErrorDetails
            {
                Message = message,
                StatusCode = (int)statusCode,
                RequestId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };

        // Don't expose sensitive information in production
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            response.Error.Details = exception.Message;
            response.Error.StackTrace = exception.StackTrace;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Standardized error response structure
/// </summary>
public class ErrorResponse
{
    public ErrorDetails Error { get; set; } = new();
}

/// <summary>
/// Error details structure
/// </summary>
public class ErrorDetails
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
}