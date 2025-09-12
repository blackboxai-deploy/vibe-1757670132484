using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Standing instruction entity for recurring payments and transfers
/// BoG compliance: Automated payment processing with proper authorization
/// </summary>
public class Standing : BaseEntity
{
    [Required]
    public Guid AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public virtual Account Account { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string InstructionReference { get; set; } = string.Empty;
    
    [Required]
    public StandingInstructionType Type { get; set; }
    
    [Required]
    public StandingInstructionStatus Status { get; set; } = StandingInstructionStatus.Active;
    
    [Required]
    [MaxLength(100)]
    public string BeneficiaryName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string BeneficiaryAccount { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? BeneficiaryBankCode { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "GHS";
    
    [Required]
    public StandingFrequency Frequency { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Day of month for monthly instructions (1-31)
    /// </summary>
    public int? DayOfMonth { get; set; }
    
    /// <summary>
    /// Day of week for weekly instructions (0=Sunday, 6=Saturday)
    /// </summary>
    public int? DayOfWeek { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Reference { get; set; }
    
    /// <summary>
    /// Next execution date
    /// </summary>
    public DateTime? NextExecutionDate { get; set; }
    
    /// <summary>
    /// Last execution date
    /// </summary>
    public DateTime? LastExecutionDate { get; set; }
    
    /// <summary>
    /// Number of times executed
    /// </summary>
    public int ExecutionCount { get; set; } = 0;
    
    /// <summary>
    /// Maximum number of executions (null for unlimited)
    /// </summary>
    public int? MaxExecutions { get; set; }
    
    /// <summary>
    /// Number of failed executions
    /// </summary>
    public int FailedExecutions { get; set; } = 0;
    
    /// <summary>
    /// Last failure reason
    /// </summary>
    [MaxLength(500)]
    public string? LastFailureReason { get; set; }
    
    /// <summary>
    /// Whether to retry failed executions
    /// </summary>
    public bool RetryOnFailure { get; set; } = true;
    
    /// <summary>
    /// Maximum number of retries
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Current retry count
    /// </summary>
    public int RetryCount { get; set; } = 0;
    
    /// <summary>
    /// User who created this instruction
    /// </summary>
    [MaxLength(100)]
    public string? CreatedByUser { get; set; }
    
    /// <summary>
    /// User who approved this instruction
    /// </summary>
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    
    /// <summary>
    /// Approval date
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    
    /// <summary>
    /// Whether this instruction requires approval
    /// </summary>
    public bool RequiresApproval { get; set; } = true;
}

/// <summary>
/// Standing instruction type enumeration
/// </summary>
public enum StandingInstructionType
{
    Transfer = 1,
    Payment = 2,
    LoanRepayment = 3,
    SavingsDeposit = 4,
    BillPayment = 5,
    DividendPayment = 6,
    SalaryPayment = 7
}

/// <summary>
/// Standing instruction status enumeration
/// </summary>
public enum StandingInstructionStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Expired = 4,
    Cancelled = 5,
    PendingApproval = 6
}

/// <summary>
/// Standing instruction frequency enumeration
/// </summary>
public enum StandingFrequency
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    SemiAnnually = 5,
    Annually = 6,
    Biweekly = 7
}