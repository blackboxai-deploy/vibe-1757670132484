using BankPlatform.API.Configuration;
using BankPlatform.API.Extensions;
using BankPlatform.API.Middleware;
using BankPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Create Serilog logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/bankplatform-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting BoG-Compliant Digital Banking Platform");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Configure Serilog
    builder.Host.UseSerilog();
    
    // Add configuration
    builder.Configuration.AddEnvironmentVariables();
    
    // Configure services
    builder.Services.ConfigureDatabase(builder.Configuration);
    builder.Services.ConfigureIdentity();
    builder.Services.ConfigureAuthentication(builder.Configuration);
    builder.Services.ConfigureAuthorization();
    builder.Services.ConfigureCaching(builder.Configuration);
    builder.Services.ConfigureRateLimiting();
    builder.Services.ConfigureHealthChecks();
    builder.Services.ConfigureOpenApi();
    builder.Services.ConfigureCors();
    builder.Services.ConfigureBusinessServices();
    builder.Services.ConfigureIntegrationServices();
    
    // Configure OpenTelemetry
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("BankPlatform.API"))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddConsoleExporter()
                .AddJaegerExporter();
        });
    
    // Add controllers and minimal APIs
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    
    var app = builder.Build();
    
    // Configure middleware pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Platform API V1");
            c.RoutePrefix = "swagger";
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
        });
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    
    // Security middleware
    app.UseHttpsRedirection();
    app.UseSecurityHeaders();
    app.UseCors();
    app.UseRateLimiter();
    
    // Custom middleware
    app.UseMiddleware<AuditLoggingMiddleware>();
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    
    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Health checks
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration
                })
            });
            
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    });
    
    // Configure API routes
    app.MapControllers();
    app.ConfigureApiRoutes();
    
    // Database migration and seeding
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BankPlatformDbContext>();
        
        if (app.Environment.IsDevelopment() || 
            builder.Configuration.GetValue<bool>("RunMigrationsOnStartup"))
        {
            await context.Database.MigrateAsync();
            await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
        }
    }
    
    Log.Information("Bank Platform API started successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}