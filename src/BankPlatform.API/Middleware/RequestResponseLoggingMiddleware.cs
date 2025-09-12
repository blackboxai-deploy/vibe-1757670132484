using System.Text;

namespace BankPlatform.API.Middleware;

/// <summary>
/// Request and response logging middleware for debugging and monitoring
/// BoG compliance: Detailed request/response logging for audit purposes
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        
        // Log request
        await LogRequestAsync(context, requestId);

        // Store original response stream
        var originalResponseBody = context.Response.Body;

        try
        {
            // Replace response stream with memory stream to capture response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Log response
            await LogResponseAsync(context, requestId, responseBody);

            // Copy captured response back to original stream
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId)
    {
        if (!ShouldLogRequest(context))
            return;

        var request = context.Request;
        var requestBody = await ReadRequestBodyAsync(request);

        var logData = new
        {
            RequestId = requestId,
            Method = request.Method,
            Path = request.Path.Value,
            QueryString = request.QueryString.Value,
            Headers = GetSafeHeaders(request.Headers),
            ContentType = request.ContentType,
            ContentLength = request.ContentLength,
            Body = SanitizeRequestBody(requestBody, request.ContentType),
            UserAgent = request.Headers.UserAgent.FirstOrDefault(),
            IPAddress = GetClientIpAddress(context),
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("HTTP Request: {@RequestData}", logData);
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, MemoryStream responseBody)
    {
        if (!ShouldLogResponse(context))
            return;

        var response = context.Response;
        var responseContent = await ReadResponseBodyAsync(responseBody);

        var logData = new
        {
            RequestId = requestId,
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength,
            Headers = GetSafeHeaders(response.Headers),
            Body = SanitizeResponseBody(responseContent, response.ContentType),
            Timestamp = DateTime.UtcNow
        };

        var logLevel = response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        _logger.Log(logLevel, "HTTP Response: {@ResponseData}", logData);
    }

    private static bool ShouldLogRequest(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        // Skip logging for certain paths
        var skipPaths = new[]
        {
            "/health",
            "/swagger",
            "/favicon.ico",
            "/_vs/browserlink",
            "/css/",
            "/js/",
            "/images/",
            "/lib/"
        };

        return !skipPaths.Any(skipPath => path?.StartsWith(skipPath) == true);
    }

    private static bool ShouldLogResponse(HttpContext context)
    {
        return ShouldLogRequest(context);
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == 0 || request.ContentLength == null)
            return string.Empty;

        // Enable request body buffering
        request.EnableBuffering();

        try
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset position for next middleware
            return body;
        }
        catch (Exception)
        {
            return "[Unable to read request body]";
        }
    }

    private static async Task<string> ReadResponseBodyAsync(MemoryStream responseBody)
    {
        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            return content;
        }
        catch (Exception)
        {
            return "[Unable to read response body]";
        }
    }

    private static string SanitizeRequestBody(string body, string? contentType)
    {
        if (string.IsNullOrEmpty(body))
            return string.Empty;

        // Don't log sensitive content
        if (contentType?.Contains("multipart/form-data") == true)
            return "[Binary/Form Data]";

        if (body.Length > 10000) // Limit body size in logs
            return body.Substring(0, 10000) + "... [truncated]";

        // Remove sensitive fields from JSON
        if (contentType?.Contains("application/json") == true)
        {
            return SanitizeJsonContent(body);
        }

        return body;
    }

    private static string SanitizeResponseBody(string body, string? contentType)
    {
        if (string.IsNullOrEmpty(body))
            return string.Empty;

        // Don't log binary content
        if (contentType?.StartsWith("image/") == true ||
            contentType?.StartsWith("video/") == true ||
            contentType?.StartsWith("audio/") == true ||
            contentType?.Contains("application/octet-stream") == true)
            return "[Binary Content]";

        if (body.Length > 10000) // Limit body size in logs
            return body.Substring(0, 10000) + "... [truncated]";

        // Remove sensitive fields from JSON responses
        if (contentType?.Contains("application/json") == true)
        {
            return SanitizeJsonContent(body);
        }

        return body;
    }

    private static string SanitizeJsonContent(string json)
    {
        try
        {
            // Simple regex-based sanitization for common sensitive fields
            var sensitiveFields = new[]
            {
                "password", "token", "secret", "key", "authorization",
                "pin", "ssn", "cardnumber", "cvv", "account_number"
            };

            var sanitized = json;
            foreach (var field in sensitiveFields)
            {
                // Replace field values with [REDACTED]
                var pattern = $"\"{field}\"\\s*:\\s*\"[^\"]*\"";
                sanitized = System.Text.RegularExpressions.Regex.Replace(
                    sanitized, pattern, $"\"{field}\": \"[REDACTED]\"", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return sanitized;
        }
        catch (Exception)
        {
            return "[Unable to sanitize JSON]";
        }
    }

    private static Dictionary<string, object> GetSafeHeaders(IHeaderDictionary headers)
    {
        var safeHeaders = new Dictionary<string, object>();
        var sensitiveHeaders = new[]
        {
            "authorization", "cookie", "x-api-key", "x-auth-token"
        };

        foreach (var header in headers)
        {
            if (sensitiveHeaders.Contains(header.Key.ToLowerInvariant()))
            {
                safeHeaders[header.Key] = "[REDACTED]";
            }
            else
            {
                safeHeaders[header.Key] = header.Value.ToString();
            }
        }

        return safeHeaders;
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString();
    }
}