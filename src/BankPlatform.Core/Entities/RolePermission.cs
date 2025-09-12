using System.ComponentModel.DataAnnotations;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Role permission entity for granular access control
/// BoG compliance: Role-based access control implementation
/// </summary>
public class RolePermission : BaseEntity
{
    [Required]
    public Guid RoleId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Permission { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Resource { get; set; }
    
    [MaxLength(50)]
    public string? Action { get; set; }
    
    /// <summary>
    /// Permission scope (Global, Branch, Customer, etc.)
    /// </summary>
    [MaxLength(50)]
    public string Scope { get; set; } = "Global";
    
    /// <summary>
    /// Conditions under which this permission applies (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? Conditions { get; set; }
    
    /// <summary>
    /// Whether this permission is granted (true) or denied (false)
    /// </summary>
    public bool IsGranted { get; set; } = true;
    
    /// <summary>
    /// Permission priority for conflict resolution
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Time-based restrictions (JSON)
    /// </summary>
    [MaxLength(500)]
    public string? TimeRestrictions { get; set; }
    
    /// <summary>
    /// IP-based restrictions (JSON array)
    /// </summary>
    [MaxLength(1000)]
    public string? IpRestrictions { get; set; }
    
    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? Metadata { get; set; }
    
    /// <summary>
    /// Permission expiry date
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// System permissions for the banking platform
/// </summary>
public static class SystemPermissions
{
    // Customer Management
    public const string CustomerCreate = "customer:create";
    public const string CustomerRead = "customer:read";
    public const string CustomerUpdate = "customer:update";
    public const string CustomerDelete = "customer:delete";
    public const string CustomerKyc = "customer:kyc";
    
    // Account Management
    public const string AccountCreate = "account:create";
    public const string AccountRead = "account:read";
    public const string AccountUpdate = "account:update";
    public const string AccountClose = "account:close";
    public const string AccountFreeze = "account:freeze";
    
    // Transaction Management
    public const string TransactionCreate = "transaction:create";
    public const string TransactionRead = "transaction:read";
    public const string TransactionReverse = "transaction:reverse";
    public const string TransactionApprove = "transaction:approve";
    
    // Loan Management
    public const string LoanCreate = "loan:create";
    public const string LoanRead = "loan:read";
    public const string LoanUpdate = "loan:update";
    public const string LoanApprove = "loan:approve";
    public const string LoanDisburse = "loan:disburse";
    
    // Administration
    public const string UserManagement = "admin:users";
    public const string RoleManagement = "admin:roles";
    public const string SystemConfiguration = "admin:config";
    public const string AuditLogs = "admin:audit";
    public const string Reports = "admin:reports";
    
    // Get all permissions
    public static IEnumerable<string> All => new[]
    {
        CustomerCreate, CustomerRead, CustomerUpdate, CustomerDelete, CustomerKyc,
        AccountCreate, AccountRead, AccountUpdate, AccountClose, AccountFreeze,
        TransactionCreate, TransactionRead, TransactionReverse, TransactionApprove,
        LoanCreate, LoanRead, LoanUpdate, LoanApprove, LoanDisburse,
        UserManagement, RoleManagement, SystemConfiguration, AuditLogs, Reports
    };
}