using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BankPlatform.Core.Entities;
using BankPlatform.Identity.Models;

namespace BankPlatform.Infrastructure.Data;

/// <summary>
/// Main database context for the banking platform
/// BoG compliance: Secure data access with audit trails
/// </summary>
public class BankPlatformDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ICurrentUserService? _currentUserService;
    
    public BankPlatformDbContext(
        DbContextOptions<BankPlatformDbContext> options,
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // Core Banking Entities
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<KycDocument> KycDocuments => Set<KycDocument>();
    
    // Audit and Compliance
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    // Identity Extensions
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<UserLoginHistory> UserLoginHistory => Set<UserLoginHistory>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    
    // Transaction Support
    public DbSet<TransactionEntry> TransactionEntries => Set<TransactionEntry>();
    public DbSet<TransactionApproval> TransactionApprovals => Set<TransactionApproval>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<MobileMoneyLink> MobileMoneyLinks => Set<MobileMoneyLink>();
    
    // Account Support
    public DbSet<AccountHolder> AccountHolders => Set<AccountHolder>();
    public DbSet<Standing> StandingInstructions => Set<Standing>();
    
    // Loan Support
    public DbSet<LoanPayment> LoanPayments => Set<LoanPayment>();
    public DbSet<LoanDocument> LoanDocuments => Set<LoanDocument>();
    public DbSet<LoanApproval> LoanApprovals => Set<LoanApproval>();
    
    // Customer Support
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure schema
        builder.HasDefaultSchema("bankplatform");
        
        // Apply entity configurations
        ApplyEntityConfigurations(builder);
        
        // Configure Identity tables
        ConfigureIdentityTables(builder);
        
        // Configure indexes for performance
        ConfigureIndexes(builder);
        
        // Configure global query filters
        ConfigureGlobalFilters(builder);
    }

    private void ApplyEntityConfigurations(ModelBuilder builder)
    {
        // Customer configuration
        builder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.CustomerNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.GhanaCardNumber).IsUnique().HasFilter("[GhanaCardNumber] IS NOT NULL");
            entity.HasIndex(e => new { e.FirstName, e.LastName });
            
            entity.Property(e => e.CustomerNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.MonthlyIncome).HasPrecision(18, 2);
        });

        // Account configuration
        builder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.AccountType });
            
            entity.Property(e => e.AccountNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.AvailableBalance).HasPrecision(18, 2);
            entity.Property(e => e.LedgerBalance).HasPrecision(18, 2);
            entity.Property(e => e.MinimumBalance).HasPrecision(18, 2);
            entity.Property(e => e.MaximumBalance).HasPrecision(18, 2);
            entity.Property(e => e.DailyTransactionLimit).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyTransactionLimit).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 4);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("GHS");
            
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Accounts)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Wallet configuration
        builder.Entity<Wallet>(entity =>
        {
            entity.HasIndex(e => e.WalletNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.MobileNumber);
            
            entity.Property(e => e.WalletNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.AvailableBalance).HasPrecision(18, 2);
            entity.Property(e => e.PendingBalance).HasPrecision(18, 2);
            entity.Property(e => e.DailyLimit).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyLimit).HasPrecision(18, 2);
            entity.Property(e => e.MaxBalance).HasPrecision(18, 2);
            entity.Property(e => e.MobileNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("GHS");
            
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Wallets)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Transaction configuration
        builder.Entity<Transaction>(entity =>
        {
            entity.HasIndex(e => e.TransactionReference).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.DebitAccountId);
            entity.HasIndex(e => e.CreditAccountId);
            entity.HasIndex(e => new { e.CreatedAt, e.Status });
            entity.HasIndex(e => e.ValueDate);
            
            entity.Property(e => e.TransactionReference).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Fee).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("GHS");
            
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.DebitAccount)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.DebitAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.CreditAccount)
                  .WithMany()
                  .HasForeignKey(e => e.CreditAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Loan configuration
        builder.Entity<Loan>(entity =>
        {
            entity.HasIndex(e => e.LoanNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.Status, e.DaysPastDue });
            entity.HasIndex(e => e.NextPaymentDate);
            
            entity.Property(e => e.LoanNumber).HasMaxLength(20).IsRequired();
            entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
            entity.Property(e => e.ApprovedAmount).HasPrecision(18, 2);
            entity.Property(e => e.DisbursedAmount).HasPrecision(18, 2);
            entity.Property(e => e.OutstandingBalance).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 4);
            entity.Property(e => e.MonthlyInstallment).HasPrecision(18, 2);
            entity.Property(e => e.CollateralValue).HasPrecision(18, 2);
            entity.Property(e => e.PenaltyAmount).HasPrecision(18, 2);
            entity.Property(e => e.ProvisionAmount).HasPrecision(18, 2);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("GHS");
            
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Loans)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Address configuration
        builder.Entity<Address>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.IsPrimary });
            
            entity.Property(e => e.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(e => e.City).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Region).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100).HasDefaultValue("Ghana");
            
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Addresses)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // KYC Document configuration
        builder.Entity<KycDocument>(entity =>
        {
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.DocumentType, e.DocumentNumber });
            
            entity.Property(e => e.DocumentNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DocumentPath).HasMaxLength(500).IsRequired();
            
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.KycDocuments)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Audit Log configuration
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
            entity.HasIndex(e => e.Action);
            
            entity.Property(e => e.UserId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(45);
        });
    }

    private void ConfigureIdentityTables(ModelBuilder builder)
    {
        // Rename Identity tables to follow naming convention
        builder.Entity<ApplicationUser>().ToTable("Users", "identity");
        builder.Entity<ApplicationRole>().ToTable("Roles", "identity");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("UserRoles", "identity");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("UserClaims", "identity");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("UserLogins", "identity");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("UserTokens", "identity");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "identity");
        
        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.CustomerId).IsUnique().HasFilter("[CustomerId] IS NOT NULL");
            
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EmployeeId).HasMaxLength(20).IsRequired();
            
            entity.HasOne(e => e.Manager)
                  .WithMany(m => m.Subordinates)
                  .HasForeignKey(e => e.ManagerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureIndexes(ModelBuilder builder)
    {
        // Performance indexes
        builder.Entity<Transaction>()
            .HasIndex(e => new { e.CreatedAt, e.Status, e.TransactionType })
            .HasDatabaseName("IX_Transaction_Performance");
            
        builder.Entity<Account>()
            .HasIndex(e => new { e.Status, e.AccountType, e.CustomerId })
            .HasDatabaseName("IX_Account_Performance");
            
        builder.Entity<Loan>()
            .HasIndex(e => new { e.Status, e.Classification, e.NextPaymentDate })
            .HasDatabaseName("IX_Loan_Performance");
    }

    private void ConfigureGlobalFilters(ModelBuilder builder)
    {
        // Soft delete filter
        builder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Account>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Wallet>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Transaction>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Loan>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var currentUser = _currentUserService?.GetCurrentUserId() ?? "System";
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUser;
                    break;
            }
        }
    }
}

/// <summary>
/// Service for getting current user information
/// </summary>
public interface ICurrentUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserName();
    bool IsAuthenticated();
}