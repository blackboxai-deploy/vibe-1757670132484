using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Loan approval entity for multi-level loan approval workflow
/// BoG compliance: Maker-checker approval process for loans
/// </summary>
public class LoanApproval : BaseEntity
{
    [Required]
    public Guid LoanId { get; set; }
    
    [ForeignKey(nameof(LoanId))]
    public virtual Loan Loan { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string ApproverId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string ApproverName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? ApproverRole { get; set; }
    
    [Required]
    public ApprovalLevel ApprovalLevel { get; set; }
    
    [Required]
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    
    [Required]
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    
    [MaxLength(1000)]
    public string? Comments { get; set; }
    
    [MaxLength(500)]
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Approved loan amount (may differ from requested amount)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ApprovedAmount { get; set; }
    
    /// <summary>
    /// Approved interest rate
    /// </summary>
    [Column(TypeName = "decimal(5,4)")]
    public decimal? ApprovedInterestRate { get; set; }
    
    /// <summary>
    /// Approved tenure in months
    /// </summary>
    public int? ApprovedTenure { get; set; }
    
    /// <summary>
    /// Special conditions or terms
    /// </summary>
    [MaxLength(1000)]
    public string? SpecialConditions { get; set; }
    
    /// <summary>
    /// Whether this approval level is required
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// Sequence order of approval
    /// </summary>
    public int Sequence { get; set; } = 1;
    
    /// <summary>
    /// Deadline for approval decision
    /// </summary>
    public DateTime? ApprovalDeadline { get; set; }
    
    /// <summary>
    /// IP address from which approval was made
    /// </summary>
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent information
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Digital signature or approval code
    /// </summary>
    [MaxLength(500)]
    public string? DigitalSignature { get; set; }
    
    /// <summary>
    /// Risk assessment score at time of approval
    /// </summary>
    public decimal? RiskScore { get; set; }
    
    /// <summary>
    /// Risk assessment comments
    /// </summary>
    [MaxLength(500)]
    public string? RiskComments { get; set; }
    
    /// <summary>
    /// Collateral valuation at time of approval
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? CollateralValuation { get; set; }
    
    /// <summary>
    /// Loan-to-value ratio
    /// </summary>
    [Column(TypeName = "decimal(5,4)")]
    public decimal? LoanToValueRatio { get; set; }
    
    /// <summary>
    /// Debt-to-income ratio
    /// </summary>
    [Column(TypeName = "decimal(5,4)")]
    public decimal? DebtToIncomeRatio { get; set; }
    
    /// <summary>
    /// Whether additional documentation is required
    /// </summary>
    public bool RequiresAdditionalDocuments { get; set; } = false;
    
    /// <summary>
    /// List of additional documents required (JSON)
    /// </summary>
    [MaxLength(1000)]
    public string? AdditionalDocumentsRequired { get; set; }
    
    /// <summary>
    /// Escalation reason if escalated to higher level
    /// </summary>
    [MaxLength(500)]
    public string? EscalationReason { get; set; }
    
    /// <summary>
    /// Whether this approval was escalated
    /// </summary>
    public bool IsEscalated { get; set; } = false;
}

/// <summary>
/// Approval level enumeration
/// </summary>
public enum ApprovalLevel
{
    FirstLevel = 1,      // Branch Manager/Loan Officer
    SecondLevel = 2,     // Regional Manager
    ThirdLevel = 3,      // Credit Committee
    FinalLevel = 4,      // CEO/Board Level
    AutoApproval = 5     // System auto-approval for small amounts
}

/// <summary>
/// Approval status enumeration (extending the base ApprovalStatus)
/// </summary>
public enum LoanApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Expired = 4,
    Withdrawn = 5,
    OnHold = 6,
    RequiresMoreInfo = 7,
    Escalated = 8,
    ConditionallyApproved = 9
}