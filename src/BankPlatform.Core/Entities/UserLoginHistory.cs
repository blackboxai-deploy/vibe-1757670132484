using System.ComponentModel.DataAnnotations;

namespace BankPlatform.Core.Entities;

/// <summary>
/// User login history for security tracking and compliance
/// BoG compliance: Authentication audit trail
/// </summary>
public class UserLoginHistory : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    public LoginResult LoginResult { get; set; }
    
    [Required]
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// IP address from which login was attempted
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string
    /// </summary>
    [MaxLength(1000)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Device information
    /// </summary>
    [MaxLength(200)]
    public string? DeviceInfo { get; set; }
    
    /// <summary>
    /// Geographic location of login attempt
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Reason for failed login
    /// </summary>
    [MaxLength(200)]
    public string? FailureReason { get; set; }
    
    /// <summary>
    /// Whether two-factor authentication was used
    /// </summary>
    public bool TwoFactorUsed { get; set; } = false;
    
    /// <summary>
    /// Type of two-factor authentication used
    /// </summary>
    [MaxLength(50)]
    public string? TwoFactorMethod { get; set; }
    
    /// <summary>
    /// Session ID if login was successful
    /// </summary>
    public Guid? SessionId { get; set; }
    
    /// <summary>
    /// Whether this login was from a trusted device
    /// </summary>
    public bool FromTrustedDevice { get; set; } = false;
    
    /// <summary>
    /// Risk score assigned to this login attempt
    /// </summary>
    public decimal? RiskScore { get; set; }
    
    /// <summary>
    /// Whether additional verification was required
    /// </summary>
    public bool AdditionalVerificationRequired { get; set; } = false;
}

/// <summary>
/// Login result enumeration
/// </summary>
public enum LoginResult
{
    Success = 1,
    InvalidCredentials = 2,
    AccountLocked = 3,
    AccountDisabled = 4,
    TwoFactorRequired = 5,
    TwoFactorFailed = 6,
    DeviceNotTrusted = 7,
    RiskCheckFailed = 8,
    AdditionalVerificationRequired = 9,
    SessionExpired = 10
}