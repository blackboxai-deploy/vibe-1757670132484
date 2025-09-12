using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// User session entity for tracking active sessions
/// Security compliance: Session management and tracking
/// </summary>
public class UserSession : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string SessionToken { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string RefreshToken { get; set; } = string.Empty;
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    public DateTime? RefreshTokenExpiresAt { get; set; }
    
    /// <summary>
    /// IP address from which session was created
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent string for device identification
    /// </summary>
    [MaxLength(1000)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Device fingerprint for security
    /// </summary>
    [MaxLength(500)]
    public string? DeviceFingerprint { get; set; }
    
    /// <summary>
    /// Geographic location (city, country)
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Session termination timestamp
    /// </summary>
    public DateTime? TerminatedAt { get; set; }
    
    /// <summary>
    /// Reason for session termination
    /// </summary>
    [MaxLength(100)]
    public string? TerminationReason { get; set; }
    
    /// <summary>
    /// Whether session is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether this session has elevated privileges
    /// </summary>
    public bool HasElevatedAccess { get; set; } = false;
    
    /// <summary>
    /// Timestamp when elevated access was granted
    /// </summary>
    public DateTime? ElevatedAccessGrantedAt { get; set; }
    
    /// <summary>
    /// Timestamp when elevated access expires
    /// </summary>
    public DateTime? ElevatedAccessExpiresAt { get; set; }
}