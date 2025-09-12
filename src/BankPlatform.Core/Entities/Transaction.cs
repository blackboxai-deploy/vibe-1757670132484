using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Transaction entity for all financial transactions
/// BoG compliance: Transaction monitoring and audit trail
/// </summary>
public class Transaction : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string TransactionReference { get; set; } = string.Empty;
    
    [Required]
    public TransactionType TransactionType { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "GHS";
    
    [Required]
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Narration { get; set; }
    
    /// <summary>
    /// Value date for transaction posting
    /// </summary>
    public DateTime ValueDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Transaction initiation date
    /// </summary>
    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Transaction completion date
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Channel through which transaction was initiated
    /// </summary>
    public TransactionChannel Channel { get; set; }
    
    /// <summary>
    /// External transaction reference (from payment gateways)
    /// </summary>
    [MaxLength(100)]
    public string? ExternalReference { get; set; }
    
    /// <summary>
    /// Batch reference for bulk transactions
    /// </summary>
    [MaxLength(50)]
    public string? BatchReference { get; set; }
    
    /// <summary>
    /// Reversible transaction flag
    /// </summary>
    public bool IsReversible { get; set; } = true;
    
    /// <summary>
    /// Original transaction ID for reversals
    /// </summary>
    public Guid? OriginalTransactionId { get; set; }
    
    /// <summary>
    /// Reversal transaction ID if this transaction was reversed
    /// </summary>
    public Guid? ReversalTransactionId { get; set; }
    
    /// <summary>
    /// Maker user ID for maker-checker workflow
    /// </summary>
    [MaxLength(100)]
    public string? MakerId { get; set; }
    
    /// <summary>
    /// Checker user ID for maker-checker workflow
    /// </summary>
    [MaxLength(100)]
    public string? CheckerId { get; set; }
    
    /// <summary>
    /// Authorization code from payment processor
    /// </summary>
    [MaxLength(50)]
    public string? AuthorizationCode { get; set; }
    
    /// <summary>
    /// Risk score for fraud detection
    /// </summary>
    public decimal? RiskScore { get; set; }
    
    /// <summary>
    /// Fraud check status
    /// </summary>
    public FraudStatus FraudStatus { get; set; } = FraudStatus.NotChecked;
    
    // Account relationships
    public Guid? DebitAccountId { get; set; }
    [ForeignKey(nameof(DebitAccountId))]
    public virtual Account? DebitAccount { get; set; }
    
    public Guid? CreditAccountId { get; set; }
    [ForeignKey(nameof(CreditAccountId))]
    public virtual Account? CreditAccount { get; set; }
    
    // Customer relationship
    [Required]
    public Guid CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    // Navigation properties
    public virtual ICollection<TransactionEntry> TransactionEntries { get; set; } = new List<TransactionEntry>();
    public virtual ICollection<TransactionApproval> Approvals { get; set; } = new List<TransactionApproval>();
    
    /// <summary>
    /// Balance after transaction for account reconciliation
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? BalanceAfter { get; set; }
    
    /// <summary>
    /// Location where transaction was initiated (IP address, GPS coordinates)
    /// </summary>
    [MaxLength(200)]
    public string? TransactionLocation { get; set; }
    
    /// <summary>
    /// Device information for mobile transactions
    /// </summary>
    [MaxLength(500)]
    public string? DeviceInfo { get; set; }
}