using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Bank account entity with mirror wallet support
/// BoG compliance: Account management and transaction tracking
/// </summary>
public class Account : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Required]
    public AccountType AccountType { get; set; }
    
    [Required]
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableBalance { get; set; } = 0;
    
    /// <summary>
    /// Ledger balance for reconciliation
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal LedgerBalance { get; set; } = 0;
    
    /// <summary>
    /// Minimum balance requirement
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinimumBalance { get; set; } = 0;
    
    /// <summary>
    /// Maximum balance limit for risk management
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaximumBalance { get; set; }
    
    /// <summary>
    /// Daily transaction limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DailyTransactionLimit { get; set; } = 50000; // GHS 50,000 default
    
    /// <summary>
    /// Monthly transaction limit
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyTransactionLimit { get; set; } = 500000; // GHS 500,000 default
    
    /// <summary>
    /// Interest rate for savings/fixed deposit accounts
    /// </summary>
    [Column(TypeName = "decimal(5,4)")]
    public decimal? InterestRate { get; set; }
    
    /// <summary>
    /// Account opening date
    /// </summary>
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Account closure date (if closed)
    /// </summary>
    public DateTime? ClosedAt { get; set; }
    
    /// <summary>
    /// Reason for account closure
    /// </summary>
    [MaxLength(500)]
    public string? ClosureReason { get; set; }
    
    /// <summary>
    /// Joint account flag
    /// </summary>
    public bool IsJointAccount { get; set; } = false;
    
    /// <summary>
    /// Dormant account flag for inactive accounts
    /// </summary>
    public bool IsDormant { get; set; } = false;
    
    /// <summary>
    /// Last activity date for dormancy tracking
    /// </summary>
    public DateTime? LastActivityAt { get; set; }
    
    /// <summary>
    /// Branch code where account was opened
    /// </summary>
    [MaxLength(10)]
    public string BranchCode { get; set; } = "001"; // Default branch
    
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
    /// Mirrored wallet ID for digital transactions
    /// </summary>
    public Guid? MirroredWalletId { get; set; }
    
    [ForeignKey(nameof(MirroredWalletId))]
    public virtual Wallet? MirroredWallet { get; set; }
    
    // Navigation properties
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<AccountHolder> AccountHolders { get; set; } = new List<AccountHolder>();
    public virtual ICollection<Standing> StandingInstructions { get; set; } = new List<Standing>();
}