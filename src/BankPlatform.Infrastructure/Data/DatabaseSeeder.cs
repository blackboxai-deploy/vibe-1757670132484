using BankPlatform.Core.Entities;
using BankPlatform.Core.Enums;
using BankPlatform.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankPlatform.Infrastructure.Data;

/// <summary>
/// Database seeder for initial system setup and demo data
/// BoG compliance: Ensures proper role and permission setup
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seed database with initial roles, users, and demo data
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<BankPlatformDbContext>>();
        var context = serviceProvider.GetRequiredService<BankPlatformDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        try
        {
            logger.LogInformation("Starting database seeding...");

            await SeedRolesAsync(roleManager, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedSystemPermissionsAsync(context, logger);
            await SeedDemoCustomersAsync(context, logger);
            await SeedSystemConfigurationAsync(context, logger);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Seed system roles
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        logger.LogInformation("Seeding system roles...");

        var roles = new[]
        {
            new ApplicationRole 
            { 
                Name = SystemRoles.SuperAdmin, 
                Description = "Super Administrator with full system access",
                Category = "Administrative",
                Priority = 1000,
                IsSystemRole = true,
                RequiresMakerChecker = false,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.Admin, 
                Description = "System Administrator",
                Category = "Administrative",
                Priority = 900,
                IsSystemRole = true,
                RequiresMakerChecker = true,
                MaxSessionDurationMinutes = 360 // 6 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.BankManager, 
                Description = "Bank Manager with oversight responsibilities",
                Category = "Management",
                Priority = 800,
                IsSystemRole = true,
                RequiresMakerChecker = true,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.BranchManager, 
                Description = "Branch Manager for branch operations",
                Category = "Management",
                Priority = 700,
                IsSystemRole = true,
                RequiresMakerChecker = true,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.LoanOfficer, 
                Description = "Loan Officer for loan processing",
                Category = "Banking",
                Priority = 600,
                IsSystemRole = true,
                RequiresMakerChecker = true,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.Teller, 
                Description = "Bank Teller for customer transactions",
                Category = "Banking",
                Priority = 500,
                IsSystemRole = true,
                RequiresMakerChecker = true,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.CustomerService, 
                Description = "Customer Service Representative",
                Category = "Customer Support",
                Priority = 400,
                IsSystemRole = true,
                RequiresMakerChecker = false,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.Auditor, 
                Description = "System Auditor for compliance monitoring",
                Category = "Compliance",
                Priority = 300,
                IsSystemRole = true,
                RequiresMakerChecker = false,
                MaxSessionDurationMinutes = 240 // 4 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.ComplianceOfficer, 
                Description = "Compliance Officer for regulatory oversight",
                Category = "Compliance",
                Priority = 350,
                IsSystemRole = true,
                RequiresMakerChecker = false,
                MaxSessionDurationMinutes = 480 // 8 hours
            },
            new ApplicationRole 
            { 
                Name = SystemRoles.Customer, 
                Description = "Bank Customer",
                Category = "Customer",
                Priority = 100,
                IsSystemRole = true,
                RequiresMakerChecker = false,
                MaxSessionDurationMinutes = 120 // 2 hours
            }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", role.Name);
                }
                else
                {
                    logger.LogError("Failed to create role {RoleName}: {Errors}", 
                        role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    /// <summary>
    /// Seed default system users
    /// </summary>
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        logger.LogInformation("Seeding default system users...");

        // Super Admin User
        if (await userManager.FindByEmailAsync("superadmin@bankplatform.com") == null)
        {
            var superAdmin = new ApplicationUser
            {
                UserName = "superadmin@bankplatform.com",
                Email = "superadmin@bankplatform.com",
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Administrator",
                EmployeeId = "EMP000001",
                Status = UserStatus.Active,
                Department = "IT",
                JobTitle = "Super Administrator",
                BranchCode = "HEAD",
                CreatedBy = "SYSTEM",
                SessionTimeoutMinutes = 480
            };

            var result = await userManager.CreateAsync(superAdmin, "SuperAdmin123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, SystemRoles.SuperAdmin);
                logger.LogInformation("Created Super Admin user");
            }
            else
            {
                logger.LogError("Failed to create Super Admin: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Bank Manager User
        if (await userManager.FindByEmailAsync("manager@bankplatform.com") == null)
        {
            var manager = new ApplicationUser
            {
                UserName = "manager@bankplatform.com",
                Email = "manager@bankplatform.com",
                EmailConfirmed = true,
                FirstName = "Bank",
                LastName = "Manager",
                EmployeeId = "EMP000002",
                Status = UserStatus.Active,
                Department = "Operations",
                JobTitle = "Bank Manager",
                BranchCode = "HEAD",
                CreatedBy = "SYSTEM",
                SessionTimeoutMinutes = 480
            };

            var result = await userManager.CreateAsync(manager, "BankManager123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(manager, SystemRoles.BankManager);
                logger.LogInformation("Created Bank Manager user");
            }
        }

        // Demo Teller User
        if (await userManager.FindByEmailAsync("teller@bankplatform.com") == null)
        {
            var teller = new ApplicationUser
            {
                UserName = "teller@bankplatform.com",
                Email = "teller@bankplatform.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Teller",
                EmployeeId = "EMP000003",
                Status = UserStatus.Active,
                Department = "Banking",
                JobTitle = "Senior Teller",
                BranchCode = "BR001",
                CreatedBy = "SYSTEM",
                SessionTimeoutMinutes = 480
            };

            var result = await userManager.CreateAsync(teller, "Teller123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teller, SystemRoles.Teller);
                logger.LogInformation("Created Teller user");
            }
        }

        // Demo Customer User
        if (await userManager.FindByEmailAsync("customer@example.com") == null)
        {
            var customer = new ApplicationUser
            {
                UserName = "customer@example.com",
                Email = "customer@example.com",
                EmailConfirmed = true,
                FirstName = "Demo",
                LastName = "Customer",
                EmployeeId = "CUST000001",
                Status = UserStatus.Active,
                CreatedBy = "SYSTEM",
                SessionTimeoutMinutes = 120
            };

            var result = await userManager.CreateAsync(customer, "Customer123!@#");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customer, SystemRoles.Customer);
                logger.LogInformation("Created Demo Customer user");
            }
        }
    }

    /// <summary>
    /// Seed role permissions
    /// </summary>
    private static async Task SeedSystemPermissionsAsync(BankPlatformDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding system permissions...");

        if (!context.RolePermissions.Any())
        {
            var roles = await context.Roles.ToListAsync();
            var permissions = new List<RolePermission>();

            // Super Admin gets all permissions
            var superAdminRole = roles.FirstOrDefault(r => r.Name == SystemRoles.SuperAdmin);
            if (superAdminRole != null)
            {
                foreach (var permission in SystemPermissions.All)
                {
                    permissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        Permission = permission,
                        IsGranted = true,
                        Priority = 1000,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            // Bank Manager permissions
            var managerRole = roles.FirstOrDefault(r => r.Name == SystemRoles.BankManager);
            if (managerRole != null)
            {
                var managerPermissions = new[]
                {
                    SystemPermissions.CustomerRead,
                    SystemPermissions.CustomerUpdate,
                    SystemPermissions.AccountRead,
                    SystemPermissions.AccountCreate,
                    SystemPermissions.AccountUpdate,
                    SystemPermissions.TransactionRead,
                    SystemPermissions.TransactionApprove,
                    SystemPermissions.LoanRead,
                    SystemPermissions.LoanApprove,
                    SystemPermissions.Reports
                };

                foreach (var permission in managerPermissions)
                {
                    permissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        Permission = permission,
                        IsGranted = true,
                        Priority = 900,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            // Teller permissions
            var tellerRole = roles.FirstOrDefault(r => r.Name == SystemRoles.Teller);
            if (tellerRole != null)
            {
                var tellerPermissions = new[]
                {
                    SystemPermissions.CustomerRead,
                    SystemPermissions.CustomerCreate,
                    SystemPermissions.AccountRead,
                    SystemPermissions.AccountCreate,
                    SystemPermissions.TransactionCreate,
                    SystemPermissions.TransactionRead
                };

                foreach (var permission in tellerPermissions)
                {
                    permissions.Add(new RolePermission
                    {
                        RoleId = tellerRole.Id,
                        Permission = permission,
                        IsGranted = true,
                        Priority = 500,
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            // Customer permissions
            var customerRole = roles.FirstOrDefault(r => r.Name == SystemRoles.Customer);
            if (customerRole != null)
            {
                var customerPermissions = new[]
                {
                    SystemPermissions.AccountRead,
                    SystemPermissions.TransactionRead,
                    SystemPermissions.TransactionCreate
                };

                foreach (var permission in customerPermissions)
                {
                    permissions.Add(new RolePermission
                    {
                        RoleId = customerRole.Id,
                        Permission = permission,
                        IsGranted = true,
                        Priority = 100,
                        Scope = "OwnData", // Customers can only access their own data
                        CreatedBy = "SYSTEM"
                    });
                }
            }

            context.RolePermissions.AddRange(permissions);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} role permissions", permissions.Count);
        }
    }

    /// <summary>
    /// Seed demo customers for testing
    /// </summary>
    private static async Task SeedDemoCustomersAsync(BankPlatformDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding demo customers...");

        if (!context.Customers.Any())
        {
            var customers = new[]
            {
                new Customer
                {
                    CustomerNumber = "CUST000001",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+233201234567",
                    DateOfBirth = new DateTime(1985, 6, 15),
                    Gender = Gender.Male,
                    MaritalStatus = MaritalStatus.Married,
                    GhanaCardNumber = "GHA-123456789-1",
                    Occupation = "Software Engineer",
                    MonthlyIncome = 5000,
                    IncomeSource = "Employment",
                    KycStatus = KycStatus.Completed,
                    RiskRating = RiskRating.Low,
                    CreatedBy = "SYSTEM"
                },
                new Customer
                {
                    CustomerNumber = "CUST000002",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "+233207654321",
                    DateOfBirth = new DateTime(1990, 3, 22),
                    Gender = Gender.Female,
                    MaritalStatus = MaritalStatus.Single,
                    GhanaCardNumber = "GHA-987654321-2",
                    Occupation = "Teacher",
                    MonthlyIncome = 2500,
                    IncomeSource = "Employment",
                    KycStatus = KycStatus.Completed,
                    RiskRating = RiskRating.Low,
                    CreatedBy = "SYSTEM"
                }
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} demo customers", customers.Length);
        }
    }

    /// <summary>
    /// Seed system configuration and audit logs
    /// </summary>
    private static async Task SeedSystemConfigurationAsync(BankPlatformDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding system configuration...");

        // Add initial audit log entry
        if (!context.AuditLogs.Any())
        {
            var systemAudit = new AuditLog
            {
                UserId = "SYSTEM",
                UserName = "System",
                Action = "SystemInitialization",
                EntityName = "System",
                AuditType = AuditType.SystemAction,
                IsSuccess = true,
                Module = "DatabaseSeeder",
                RiskLevel = RiskLevel.Low,
                AdditionalInfo = "Initial system setup and database seeding completed",
                CreatedBy = "SYSTEM"
            };

            context.AuditLogs.Add(systemAudit);
            await context.SaveChangesAsync();
            logger.LogInformation("Added system initialization audit log");
        }
    }
}