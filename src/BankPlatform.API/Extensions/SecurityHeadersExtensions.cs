namespace BankPlatform.API.Extensions;

/// <summary>
/// Security headers extensions for enhanced web security
/// BoG compliance: Security headers implementation following security best practices
/// </summary>
public static class SecurityHeadersExtensions
{
    /// <summary>
    /// Add comprehensive security headers middleware
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            // Remove server information headers
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            // Content Security Policy (CSP)
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://unpkg.com; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net; " +
                "img-src 'self' data: https:; " +
                "connect-src 'self' https: wss:; " +
                "media-src 'self'; " +
                "frame-src 'none'; " +
                "object-src 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self'");

            // X-Content-Type-Options
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // X-Frame-Options
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // X-XSS-Protection
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Referrer Policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions Policy (formerly Feature Policy)
            context.Response.Headers.Append("Permissions-Policy",
                "geolocation=(), " +
                "microphone=(), " +
                "camera=(), " +
                "payment=(), " +
                "usb=(), " +
                "magnetometer=(), " +
                "gyroscope=(), " +
                "speaker=()");

            // Strict Transport Security (HSTS)
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append("Strict-Transport-Security",
                    "max-age=31536000; includeSubDomains; preload");
            }

            // Cross-Origin Resource Policy
            context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-site");

            // Cross-Origin Embedder Policy
            context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");

            // Cross-Origin Opener Policy
            context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");

            // Cache Control for sensitive endpoints
            if (IsSensitiveEndpoint(context.Request.Path))
            {
                context.Response.Headers.Append("Cache-Control",
                    "no-store, no-cache, must-revalidate, private");
                context.Response.Headers.Append("Pragma", "no-cache");
                context.Response.Headers.Append("Expires", "0");
            }

            // Custom security headers
            context.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow, nosnippet, noarchive");
            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

            await next();
        });

        return app;
    }

    /// <summary>
    /// Determine if an endpoint contains sensitive information
    /// </summary>
    private static bool IsSensitiveEndpoint(PathString path)
    {
        var sensitivePatterns = new[]
        {
            "/api/auth",
            "/api/customers",
            "/api/accounts",
            "/api/transactions",
            "/api/loans",
            "/api/admin"
        };

        var pathValue = path.Value?.ToLowerInvariant();
        return sensitivePatterns.Any(pattern => pathValue?.StartsWith(pattern) == true);
    }
}

/// <summary>
/// API route configuration extensions
/// </summary>
public static class ApiRouteExtensions
{
    /// <summary>
    /// Configure API routes for the banking platform
    /// </summary>
    public static void ConfigureApiRoutes(this WebApplication app)
    {
        // API versioning
        var apiV1 = app.MapGroup("/api/v1");
        
        // Health checks
        app.MapGet("/health", () => Results.Ok(new { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        }));

        app.MapGet("/health/ready", () => Results.Ok(new { 
            status = "Ready", 
            timestamp = DateTime.UtcNow 
        }));

        app.MapGet("/health/live", () => Results.Ok(new { 
            status = "Live", 
            timestamp = DateTime.UtcNow 
        }));

        // Version endpoint
        app.MapGet("/version", () => Results.Ok(new
        {
            version = "1.0.0",
            buildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            dotNetVersion = Environment.Version.ToString()
        }));

        // API information endpoint
        apiV1.MapGet("/info", () => Results.Ok(new
        {
            name = "BoG-Compliant Digital Banking Platform API",
            version = "1.0.0",
            description = "Comprehensive banking platform API with BoG CISD compliance",
            contact = new
            {
                name = "API Support",
                email = "api-support@bankplatform.com",
                url = "https://docs.bankplatform.com"
            },
            license = new
            {
                name = "Proprietary",
                url = "https://bankplatform.com/license"
            },
            documentation = new
            {
                swagger = "/swagger",
                openApi = "/swagger/v1/swagger.json"
            }
        }));

        // Security test endpoints (for development only)
        if (app.Environment.IsDevelopment())
        {
            app.MapGet("/security/headers", (HttpContext context) =>
            {
                var headers = context.Response.Headers
                    .ToDictionary(h => h.Key, h => h.Value.ToString());
                return Results.Ok(new { securityHeaders = headers });
            });
        }

        // Favicon to prevent 404s
        app.MapGet("/favicon.ico", () => Results.NotFound());

        // Robots.txt for security
        app.MapGet("/robots.txt", () => Results.Text(
            "User-agent: *\nDisallow: /\nCrawl-delay: 86400"));

        // Security.txt for responsible disclosure
        app.MapGet("/.well-known/security.txt", () => Results.Text(
            "Contact: mailto:security@bankplatform.com\n" +
            "Encryption: https://bankplatform.com/pgp-key.txt\n" +
            "Acknowledgments: https://bankplatform.com/security/hall-of-fame\n" +
            "Policy: https://bankplatform.com/security/policy\n" +
            "Hiring: https://bankplatform.com/careers\n" +
            $"Expires: {DateTime.UtcNow.AddYears(1):yyyy-MM-ddTHH:mm:ssZ}\n"));

        // CORS preflight support
        app.MapMethods("/api/{**path}", new[] { "OPTIONS" }, (string path) => 
        {
            return Results.Ok();
        });
    }
}