using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using BankPlatform.Core.Enums;

namespace BankPlatform.Identity.Models;

/// <summary>
/// Extended IdentityUser for banking application
/// BoG compliance: User management and role-based access control
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string EmployeeId { get; set; } = string.Empty;
    
    [Required]
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    /// <summary>
    /// User creation date
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// User last update date
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
    /// User profile photo path
    /// </summary>
    [MaxLength(500)]
    public string? ProfilePhotoPath { get; set; }
    
    /// <summary>
    /// Department/division
    /// </summary>
    [MaxLength(100)]
    public string? Department { get; set; }
    
    /// <summary>
    /// Job title
    /// </summary>
    [MaxLength(100)]
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Branch code for tellers and branch staff
    /// </summary>
    [MaxLength(10)]
    public string? BranchCode { get; set; }
    
    /// <summary>
    /// Manager user ID for hierarchy
    /// </summary>
    public Guid? ManagerId { get; set; }
    
    /// <summary>
    /// Two-factor authentication secret for TOTP
    /// </summary>
    [MaxLength(500)]
    public string? TwoFactorSecret { get; set; }
    
    /// <summary>
    /// MFA preference (SMS, Email, TOTP)
    /// </summary>
    public MfaMethod? PreferredMfaMethod { get; set; }
    
    /// <summary>
    /// Failed login attempts counter
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;
    
    /// <summary>
    /// Last failed login attempt timestamp
    /// </summary>
    public DateTime? LastFailedLoginAt { get; set; }
    
    /// <summary>
    /// Account lockout timestamp
    /// </summary>
    public DateTime? LockedOutAt { get; set; }
    
    /// <summary>
    /// Password expiry date for security policy
    /// </summary>
    public DateTime? PasswordExpiryDate { get; set; }
    
    /// <summary>
    /// Force password change on next login
    /// </summary>
    public bool MustChangePassword { get; set; } = false;
    
    /// <summary>
    /// Session timeout in minutes
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 30;
    
    /// <summary>
    /// IP whitelist for additional security (JSON array)
    /// </summary>
    [MaxLength(1000)]
    public string? IpWhitelist { get; set; }
    
    /// <summary>
    /// Customer reference for customer users
    /// </summary>
    public Guid? CustomerId { get; set; }
    
    /// <summary>
    /// Device registration for mobile users (JSON)
    /// </summary>
    [MaxLength(2000)]
    public string? RegisteredDevices { get; set; }
    
    /// <summary>
    /// Last password change date
    /// </summary>
    public DateTime? LastPasswordChangeAt { get; set; }
    
    /// <summary>
    /// Terms acceptance date
    /// </summary>
    public DateTime? TermsAcceptedAt { get; set; }
    
    /// <summary>
    /// Privacy policy acceptance date
    /// </summary>
    public DateTime? PrivacyPolicyAcceptedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<UserDevice> UserDevices { get; set; } = new List<UserDevice>();
    public virtual ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();
    public virtual ApplicationUser? Manager { get; set; }
    public virtual ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();
    
    /// <summary>
    /// Full name property
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Display name with middle name if available
    /// </summary>
    public string DisplayName => string.IsNullOrEmpty(MiddleName) 
        ? $"{FirstName} {LastName}".Trim()
        : $"{FirstName} {MiddleName} {LastName}".Trim();
}

/// <summary>
/// User status enumeration
/// </summary>
public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Locked = 4,
    PendingActivation = 5,
    Terminated = 6
}

/// <summary>
/// MFA method enumeration
/// </summary>
public enum MfaMethod
{
    SMS = 1,
    Email = 2,
    TOTP = 3,
    Push = 4
}