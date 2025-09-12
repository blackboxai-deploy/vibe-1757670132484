using BankPlatform.Infrastructure.Data;
using BankPlatform.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace BankPlatform.API.Configuration;

/// <summary>
/// Extension methods for configuring services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configure database services
    /// </summary>
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "PostgreSQL";
        
        services.AddDbContext<BankPlatformDbContext>(options =>
        {
            switch (databaseProvider.ToUpperInvariant())
            {
                case "POSTGRESQL":
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
                    break;
                case "MYSQL":
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    break;
                case "INMEMORY":
                    options.UseInMemoryDatabase("BankPlatformTestDb");
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
            }
            
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        return services;
    }

    /// <summary>
    /// Configure Identity services
    /// </summary>
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings - BoG compliance
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 4;

            // Lockout settings - Security compliance
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.AllowedUserNameCharacters = 
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<BankPlatformDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configure JWT Authentication
    /// </summary>
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey not configured");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Log authentication failures
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Additional token validation logic
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configure Authorization policies
    /// </summary>
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => 
                policy.RequireRole("SuperAdmin", "Admin"))
            .AddPolicy("RequireBankingRole", policy => 
                policy.RequireRole("BankManager", "BranchManager", "Teller"))
            .AddPolicy("RequireLoanOfficerRole", policy => 
                policy.RequireRole("LoanOfficer", "BankManager"))
            .AddPolicy("RequireCustomerRole", policy => 
                policy.RequireRole("Customer"))
            .AddPolicy("RequireTwoFactor", policy =>
                policy.RequireClaim("amr", "mfa"))
            .AddPolicy("RequireHighValueTransactionApproval", policy =>
                policy.RequireClaim("approval_level", "high"));

        return services;
    }

    /// <summary>
    /// Configure caching services
    /// </summary>
    public static IServiceCollection ConfigureCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis");
        
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "BankPlatform";
            });
        }
        else
        {
            services.AddMemoryCache();
        }

        return services;
    }

    /// <summary>
    /// Configure rate limiting
    /// </summary>
    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            // Global rate limiter
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter("Global", _ => 
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1000,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // Authentication endpoint rate limiter
            options.AddPolicy("AuthPolicy", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? 
                                 context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            // Transaction endpoint rate limiter
            options.AddPolicy("TransactionPolicy", context =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: context.User.Identity?.Name ?? "anonymous",
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 20
                    }));
        });

        return services;
    }

    /// <summary>
    /// Configure health checks
    /// </summary>
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<BankPlatformDbContext>("database")
            .AddCheck("memory", () =>
            {
                var allocatedBytes = GC.GetTotalMemory(false);
                var maxBytes = 1_000_000_000; // 1GB
                
                return allocatedBytes < maxBytes 
                    ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy($"Memory usage: {allocatedBytes / 1_000_000}MB")
                    : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy($"Memory usage too high: {allocatedBytes / 1_000_000}MB");
            });

        return services;
    }

    /// <summary>
    /// Configure OpenAPI/Swagger
    /// </summary>
    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BoG-Compliant Digital Banking Platform API",
                Version = "v1",
                Description = "Comprehensive banking platform API with BoG CISD compliance",
                Contact = new OpenApiContact
                {
                    Name = "Bank Platform Development Team",
                    Email = "dev@bankplatform.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Proprietary",
                    Url = new Uri("https://bankplatform.com/license")
                }
            });

            // JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Configure CORS
    /// </summary>
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins("https://localhost:5001", "https://bankplatform.com") // Configure allowed origins
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Authorization", "Content-Type", "Accept", "X-Requested-With")
                    .SetIsOriginAllowedToReturnTrue() // Only for development
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Configure business services
    /// </summary>
    public static IServiceCollection ConfigureBusinessServices(this IServiceCollection services)
    {
        // Add business service registrations here
        // services.AddScoped<ICustomerService, CustomerService>();
        // services.AddScoped<IAccountService, AccountService>();
        // services.AddScoped<ITransactionService, TransactionService>();
        // services.AddScoped<ILoanService, LoanService>();

        return services;
    }

    /// <summary>
    /// Configure integration services
    /// </summary>
    public static IServiceCollection ConfigureIntegrationServices(this IServiceCollection services)
    {
        // Add integration service registrations here
        // services.AddScoped<IGhipssService, GhipssService>();
        // services.AddScoped<IGhanaPayService, GhanaPayService>();
        // services.AddScoped<INiaService, NiaService>();

        return services;
    }
}