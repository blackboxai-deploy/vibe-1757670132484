using System.ComponentModel.DataAnnotations;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Audit log entity for compliance and security tracking
/// BoG compliance: Comprehensive audit trail for all system activities
/// </summary>
public class AuditLog : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;
    
    public Guid? EntityId { get; set; }
    
    /// <summary>
    /// Data before the change (JSON)
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// Data after the change (JSON)
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// Changed properties list
    /// </summary>
    public string? AffectedColumns { get; set; }
    
    [Required]
    public AuditType AuditType { get; set; }
    
    /// <summary>
    /// IP address of the user
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent information
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Additional context information
    /// </summary>
    [MaxLength(1000)]
    public string? AdditionalInfo { get; set; }
    
    /// <summary>
    /// Risk level of the action
    /// </summary>
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    
    /// <summary>
    /// Session identifier
    /// </summary>
    [MaxLength(100)]
    public string? SessionId { get; set; }
    
    /// <summary>
    /// Application module that generated the audit
    /// </summary>
    [MaxLength(100)]
    public string? Module { get; set; }
    
    /// <summary>
    /// Success/failure status
    /// </summary>
    public bool IsSuccess { get; set; } = true;
    
    /// <summary>
    /// Error message if action failed
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Duration of the operation in milliseconds
    /// </summary>
    public long? DurationMs { get; set; }
}

/// <summary>
/// Audit type enumeration
/// </summary>
public enum AuditType
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    View = 6,
    Export = 7,
    Import = 8,
    SystemAction = 9,
    SecurityEvent = 10
}

/// <summary>
/// Risk level enumeration for audit events
/// </summary>
public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}