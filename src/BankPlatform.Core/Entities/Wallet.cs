using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Digital wallet entity mirroring traditional accounts
/// Supports mobile money and digital payment integrations
/// </summary>
public class Wallet : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string WalletNumber { get; set; } = string.Empty;
    
    [Required]
    public WalletType WalletType { get; set; }
    
    [Required]
    public WalletStatus Status { get; set; } = WalletStatus.Active;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableBalance { get; set; } = 0;
    
    /// <summary>
    /// Pending balance from uncleared transactions
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PendingBalance { get; set; } = 0;
    
    /// <summary>
    /// Daily transaction limit for digital payments
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DailyLimit { get; set; } = 10000; // GHS 10,000 default for mobile wallets
    
    /// <summary>
    /// Monthly transaction limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyLimit { get; set; } = 100000; // GHS 100,000 default
    
    /// <summary>
    /// Maximum wallet balance limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxBalance { get; set; } = 200000; // GHS 200,000 default
    
    /// <summary>
    /// Wallet PIN hash for mobile authentication
    /// </summary>
    [MaxLength(500)]
    public string? PinHash { get; set; }
    
    /// <summary>
    /// PIN creation/update timestamp
    /// </summary>
    public DateTime? PinUpdatedAt { get; set; }
    
    /// <summary>
    /// Failed PIN attempts counter for security
    /// </summary>
    public int FailedPinAttempts { get; set; } = 0;
    
    /// <summary>
    /// Wallet lock timestamp after failed attempts
    /// </summary>
    public DateTime? LockedAt { get; set; }
    
    /// <summary>
    /// Mobile number associated with wallet
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string MobileNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// QR code for wallet transactions
    /// </summary>
    [MaxLength(500)]
    public string? QrCode { get; set; }
    
    /// <summary>
    /// Wallet activation date
    /// </summary>
    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last transaction date for activity tracking
    /// </summary>
    public DateTime? LastTransactionAt { get; set; }
    
    /// <summary>
    /// Offline transaction support flag
    /// </summary>
    public bool SupportsOfflineTransactions { get; set; } = false;
    
    /// <summary>
    /// Biometric authentication enabled
    /// </summary>
    public bool BiometricEnabled { get; set; } = false;
    
    /// <summary>
    /// Currency code (GHS for Ghana Cedis)
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "GHS";
    
    // Foreign key relationships
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Mirrored account ID for traditional banking integration
    /// </summary>
    public Guid? MirroredAccountId { get; set; }
    
    [ForeignKey(nameof(MirroredAccountId))]
    public virtual Account? MirroredAccount { get; set; }
    
    // Navigation properties
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public virtual ICollection<MobileMoneyLink> MobileMoneyLinks { get; set; } = new List<MobileMoneyLink>();
}