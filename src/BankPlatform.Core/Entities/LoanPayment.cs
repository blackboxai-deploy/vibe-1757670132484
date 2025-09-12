using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Loan payment entity for tracking loan repayments
/// BoG compliance: Loan payment tracking and reporting
/// </summary>
public class LoanPayment : BaseEntity
{
    [Required]
    public Guid LoanId { get; set; }
    
    [ForeignKey(nameof(LoanId))]
    public virtual Loan Loan { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string PaymentReference { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaymentAmount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal InterestAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PenaltyAmount { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FeesAmount { get; set; } = 0;
    
    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    public DateTime DueDate { get; set; }
    
    [Required]
    public LoanPaymentStatus Status { get; set; } = LoanPaymentStatus.Completed;
    
    [Required]
    public PaymentMethod PaymentMethod { get; set; }
    
    [MaxLength(200)]
    public string? PaymentDescription { get; set; }
    
    /// <summary>
    /// Outstanding balance after this payment
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfterPayment { get; set; }
    
    /// <summary>
    /// Number of days early/late (negative for early, positive for late)
    /// </summary>
    public int DaysFromDue { get; set; } = 0;
    
    /// <summary>
    /// Whether this is a partial payment
    /// </summary>
    public bool IsPartialPayment { get; set; } = false;
    
    /// <summary>
    /// Expected payment amount
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal ExpectedAmount { get; set; }
    
    /// <summary>
    /// Transaction ID if payment was made through banking system
    /// </summary>
    public Guid? TransactionId { get; set; }
    
    /// <summary>
    /// Source account for payment
    /// </summary>
    public Guid? SourceAccountId { get; set; }
    
    [ForeignKey(nameof(SourceAccountId))]
    public virtual Account? SourceAccount { get; set; }
    
    /// <summary>
    /// Payment processor reference
    /// </summary>
    [MaxLength(100)]
    public string? ProcessorReference { get; set; }
    
    /// <summary>
    /// User who processed this payment
    /// </summary>
    [MaxLength(100)]
    public string? ProcessedBy { get; set; }
    
    /// <summary>
    /// Payment reversal reference if reversed
    /// </summary>
    public Guid? ReversalPaymentId { get; set; }
    
    /// <summary>
    /// Whether this payment was reversed
    /// </summary>
    public bool IsReversed { get; set; } = false;
    
    /// <summary>
    /// Reason for reversal
    /// </summary>
    [MaxLength(200)]
    public string? ReversalReason { get; set; }
}

/// <summary>
/// Loan payment status enumeration
/// </summary>
public enum LoanPaymentStatus
{
    Completed = 1,
    Pending = 2,
    Failed = 3,
    Reversed = 4,
    PartiallyReversed = 5
}

/// <summary>
/// Payment method enumeration
/// </summary>
public enum PaymentMethod
{
    BankTransfer = 1,
    Cash = 2,
    Cheque = 3,
    MobileMoney = 4,
    DebitCard = 5,
    OnlinePayment = 6,
    DirectDebit = 7,
    StandingOrder = 8
}