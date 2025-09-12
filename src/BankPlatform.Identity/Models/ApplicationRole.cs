using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BankPlatform.Identity.Models;

/// <summary>
/// Extended IdentityRole for banking application
/// BoG compliance: Role-based access control with detailed permissions
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Role category for grouping (Admin, Banking, Customer, etc.)
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Priority level for role hierarchy
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Role creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Role last update date
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Created by user ID
    /// </summary>
    [MaxLength(100)]
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Updated by user ID
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Active status flag
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// System role flag (cannot be deleted)
    /// </summary>
    public bool IsSystemRole { get; set; } = false;
    
    /// <summary>
    /// Role permissions (JSON)
    /// </summary>
    [MaxLength(4000)]
    public string? Permissions { get; set; }
    
    /// <summary>
    /// Maximum session duration for this role (minutes)
    /// </summary>
    public int? MaxSessionDurationMinutes { get; set; }
    
    /// <summary>
    /// IP restrictions for this role (JSON array)
    /// </summary>
    [MaxLength(1000)]
    public string? IpRestrictions { get; set; }
    
    /// <summary>
    /// Time-based access restrictions (JSON)
    /// </summary>
    [MaxLength(500)]
    public string? TimeRestrictions { get; set; }
    
    /// <summary>
    /// Approval workflow requirements
    /// </summary>
    public bool RequiresApproval { get; set; } = false;
    
    /// <summary>
    /// Maker-checker requirements
    /// </summary>
    public bool RequiresMakerChecker { get; set; } = false;
    
    /// <summary>
    /// Transaction limits for this role (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? TransactionLimits { get; set; }
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Predefined system roles for the banking platform
/// </summary>
public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string BankManager = "BankManager";
    public const string BranchManager = "BranchManager";
    public const string LoanOfficer = "LoanOfficer";
    public const string Teller = "Teller";
    public const string CustomerService = "CustomerService";
    public const string Auditor = "Auditor";
    public const string ComplianceOfficer = "ComplianceOfficer";
    public const string Customer = "Customer";
    public const string Agent = "Agent";
    public const string Merchant = "Merchant";
    public const string ReadOnlyUser = "ReadOnlyUser";
    
    /// <summary>
    /// Get all system roles
    /// </summary>
    public static IEnumerable<string> All => new[]
    {
        SuperAdmin, Admin, BankManager, BranchManager, LoanOfficer, 
        Teller, CustomerService, Auditor, ComplianceOfficer, 
        Customer, Agent, Merchant, ReadOnlyUser
    };
    
    /// <summary>
    /// Get administrative roles
    /// </summary>
    public static IEnumerable<string> Administrative => new[]
    {
        SuperAdmin, Admin, BankManager, BranchManager
    };
    
    /// <summary>
    /// Get banking operations roles
    /// </summary>
    public static IEnumerable<string> BankingOperations => new[]
    {
        LoanOfficer, Teller, CustomerService
    };
    
    /// <summary>
    /// Get oversight roles
    /// </summary>
    public static IEnumerable<string> Oversight => new[]
    {
        Auditor, ComplianceOfficer
    };
    
    /// <summary>
    /// Get customer-facing roles
    /// </summary>
    public static IEnumerable<string> CustomerFacing => new[]
    {
        Customer, Agent, Merchant
    };
}