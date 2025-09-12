using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Mobile money link entity for linking wallets to mobile money accounts
/// Ghana market: Integration with various mobile money providers
/// </summary>
public class MobileMoneyLink : BaseEntity
{
    [Required]
    public Guid WalletId { get; set; }
    
    [ForeignKey(nameof(WalletId))]
    public virtual Wallet Wallet { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = string.Empty; // MTN, Vodafone, AirtelTigo
    
    [Required]
    [MaxLength(20)]
    public string MobileNumber { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? AccountName { get; set; }
    
    [MaxLength(50)]
    public string? AccountNumber { get; set; }
    
    [Required]
    public LinkStatus Status { get; set; } = LinkStatus.Active;
    
    /// <summary>
    /// Whether this is the primary mobile money account
    /// </summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Link verification status
    /// </summary>
    public bool IsVerified { get; set; } = false;
    
    /// <summary>
    /// Date when link was verified
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
    
    /// <summary>
    /// Verification code used
    /// </summary>
    [MaxLength(10)]
    public string? VerificationCode { get; set; }
    
    /// <summary>
    /// Last successful transaction date
    /// </summary>
    public DateTime? LastTransactionAt { get; set; }
    
    /// <summary>
    /// Daily transaction limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DailyLimit { get; set; } = 5000; // GHS 5,000 default
    
    /// <summary>
    /// Monthly transaction limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyLimit { get; set; } = 50000; // GHS 50,000 default
    
    /// <summary>
    /// Provider-specific configuration (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? ProviderConfig { get; set; }
    
    /// <summary>
    /// Link expiry date
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Reason for deactivation
    /// </summary>
    [MaxLength(200)]
    public string? DeactivationReason { get; set; }
}

/// <summary>
/// Link status enumeration
/// </summary>
public enum LinkStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Expired = 4,
    PendingVerification = 5
}