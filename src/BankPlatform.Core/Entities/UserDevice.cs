using System.ComponentModel.DataAnnotations;

namespace BankPlatform.Core.Entities;

/// <summary>
/// User device entity for mobile and web device management
/// Security compliance: Device registration and management
/// </summary>
public class UserDevice : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string DeviceId { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? DeviceName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty; // Mobile, Tablet, Desktop, Web
    
    [MaxLength(100)]
    public string? Platform { get; set; } // iOS, Android, Windows, macOS, Linux
    
    [MaxLength(100)]
    public string? OperatingSystem { get; set; }
    
    [MaxLength(50)]
    public string? OSVersion { get; set; }
    
    [MaxLength(100)]
    public string? Browser { get; set; }
    
    [MaxLength(50)]
    public string? BrowserVersion { get; set; }
    
    [MaxLength(100)]
    public string? AppVersion { get; set; }
    
    /// <summary>
    /// Device registration token for push notifications
    /// </summary>
    [MaxLength(500)]
    public string? PushToken { get; set; }
    
    /// <summary>
    /// Whether device is trusted for certain operations
    /// </summary>
    public bool IsTrusted { get; set; } = false;
    
    /// <summary>
    /// Whether device supports biometric authentication
    /// </summary>
    public bool SupportsBiometric { get; set; } = false;
    
    /// <summary>
    /// Whether biometric authentication is enabled
    /// </summary>
    public bool BiometricEnabled { get; set; } = false;
    
    /// <summary>
    /// Device fingerprint for security identification
    /// </summary>
    [MaxLength(500)]
    public string? DeviceFingerprint { get; set; }
    
    /// <summary>
    /// Last known IP address
    /// </summary>
    [MaxLength(45)]
    public string? LastKnownIP { get; set; }
    
    /// <summary>
    /// Last known location (city, country)
    /// </summary>
    [MaxLength(200)]
    public string? LastKnownLocation { get; set; }
    
    /// <summary>
    /// Last time device was used
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
    
    /// <summary>
    /// Device registration date
    /// </summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether device is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Reason for device deactivation
    /// </summary>
    [MaxLength(200)]
    public string? DeactivationReason { get; set; }
}