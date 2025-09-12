using BankPlatform.Core.Entities;
using BankPlatform.Core.Enums;
using BankPlatform.Infrastructure.Data;
using System.Text.Json;

namespace BankPlatform.API.Middleware;

/// <summary>
/// Audit logging middleware for compliance and security tracking
/// BoG compliance: Comprehensive audit trail for all API activities
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuditLoggingMiddleware(
        RequestDelegate next,
        ILogger<AuditLoggingMiddleware> logger,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            await LogAuditAsync(context, startTime, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task LogAuditAsync(HttpContext context, DateTime startTime, long durationMs)
    {
        // Skip audit logging for health checks and static files
        if (ShouldSkipAuditLogging(context))
            return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BankPlatformDbContext>();

            var auditLog = new AuditLog
            {
                UserId = GetUserId(context),
                UserName = GetUserName(context),
                Action = GetActionName(context),
                EntityName = GetEntityName(context),
                AuditType = GetAuditType(context.Request.Method),
                IpAddress = GetClientIpAddress(context),
                UserAgent = context.Request.Headers.UserAgent.FirstOrDefault(),
                SessionId = GetSessionId(context),
                Module = "API",
                IsSuccess = IsSuccessStatusCode(context.Response.StatusCode),
                ErrorMessage = IsSuccessStatusCode(context.Response.StatusCode) ? null : $"HTTP {context.Response.StatusCode}",
                DurationMs = durationMs,
                RiskLevel = DetermineRiskLevel(context),
                AdditionalInfo = GetAdditionalInfo(context)
            };

            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Audit: {UserId} performed {Action} on {EntityName} from {IpAddress} - Status: {StatusCode} Duration: {Duration}ms",
                auditLog.UserId, auditLog.Action, auditLog.EntityName, auditLog.IpAddress, 
                context.Response.StatusCode, durationMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit information for request {RequestId}", 
                context.TraceIdentifier);
        }
    }

    private static bool ShouldSkipAuditLogging(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        var skipPaths = new[]
        {
            "/health",
            "/swagger",
            "/favicon.ico",
            "/_vs/browserlink",
            "/css/",
            "/js/",
            "/images/"
        };

        return skipPaths.Any(skipPath => path?.StartsWith(skipPath) == true);
    }

    private static string GetUserId(HttpContext context)
    {
        return context.User?.FindFirst("sub")?.Value 
            ?? context.User?.FindFirst("id")?.Value 
            ?? "Anonymous";
    }

    private static string GetUserName(HttpContext context)
    {
        return context.User?.Identity?.Name 
            ?? context.User?.FindFirst("name")?.Value 
            ?? "Anonymous";
    }

    private static string GetActionName(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";
        
        // Extract meaningful action names
        if (path.Contains("/auth/", StringComparison.OrdinalIgnoreCase))
            return "Authentication";
        if (path.Contains("/customers/", StringComparison.OrdinalIgnoreCase))
            return $"Customer_{method}";
        if (path.Contains("/accounts/", StringComparison.OrdinalIgnoreCase))
            return $"Account_{method}";
        if (path.Contains("/transactions/", StringComparison.OrdinalIgnoreCase))
            return $"Transaction_{method}";
        if (path.Contains("/loans/", StringComparison.OrdinalIgnoreCase))
            return $"Loan_{method}";

        return $"{method}_{path}";
    }

    private static string GetEntityName(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        
        if (path.Contains("/customers")) return "Customer";
        if (path.Contains("/accounts")) return "Account";
        if (path.Contains("/transactions")) return "Transaction";
        if (path.Contains("/loans")) return "Loan";
        if (path.Contains("/wallets")) return "Wallet";
        if (path.Contains("/auth")) return "Authentication";
        
        return "Unknown";
    }

    private static AuditType GetAuditType(string httpMethod)
    {
        return httpMethod.ToUpperInvariant() switch
        {
            "POST" => AuditType.Create,
            "PUT" or "PATCH" => AuditType.Update,
            "DELETE" => AuditType.Delete,
            "GET" => AuditType.View,
            _ => AuditType.SystemAction
        };
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetSessionId(HttpContext context)
    {
        return context.Request.Headers["X-Session-Id"].FirstOrDefault()
            ?? context.Session?.Id;
    }

    private static bool IsSuccessStatusCode(int statusCode)
    {
        return statusCode >= 200 && statusCode < 400;
    }

    private static RiskLevel DetermineRiskLevel(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var method = context.Request.Method.ToUpperInvariant();

        // High-risk operations
        if (path.Contains("/transactions") && (method == "POST" || method == "PUT"))
            return RiskLevel.High;
        if (path.Contains("/loans") && (method == "POST" || method == "PUT"))
            return RiskLevel.High;
        if (path.Contains("/auth/login") || path.Contains("/auth/register"))
            return RiskLevel.Medium;

        // Failed authentication attempts
        if (!IsSuccessStatusCode(context.Response.StatusCode) && path.Contains("/auth"))
            return RiskLevel.High;

        return RiskLevel.Low;
    }

    private static string GetAdditionalInfo(HttpContext context)
    {
        var info = new
        {
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.Value,
            ContentType = context.Request.ContentType,
            ContentLength = context.Request.ContentLength,
            StatusCode = context.Response.StatusCode,
            UserAgent = context.Request.Headers.UserAgent.FirstOrDefault()
        };

        return JsonSerializer.Serialize(info, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}