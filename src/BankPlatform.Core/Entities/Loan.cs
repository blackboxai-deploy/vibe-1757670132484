using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Loan entity for individual and group lending
/// BoG compliance: Loan management and reporting requirements
/// </summary>
public class Loan : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string LoanNumber { get; set; } = string.Empty;
    
    [Required]
    public LoanType LoanType { get; set; }
    
    [Required]
    public LoanStatus Status { get; set; } = LoanStatus.Applied;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ApprovedAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DisbursedAmount { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal OutstandingBalance { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(5,4)")]
    public decimal InterestRate { get; set; } // Annual percentage rate
    
    [Required]
    public int TenureMonths { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyInstallment { get; set; }
    
    /// <summary>
    /// Loan application date
    /// </summary>
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Loan approval date
    /// </summary>
    public DateTime? ApprovalDate { get; set; }
    
    /// <summary>
    /// Loan disbursement date
    /// </summary>
    public DateTime? DisbursementDate { get; set; }
    
    /// <summary>
    /// Expected maturity date
    /// </summary>
    public DateTime? MaturityDate { get; set; }
    
    /// <summary>
    /// Actual closure date
    /// </summary>
    public DateTime? ClosureDate { get; set; }
    
    /// <summary>
    /// Next payment due date
    /// </summary>
    public DateTime? NextPaymentDate { get; set; }
    
    /// <summary>
    /// Last payment date
    /// </summary>
    public DateTime? LastPaymentDate { get; set; }
    
    /// <summary>
    /// Purpose of the loan
    /// </summary>
    [MaxLength(500)]
    public string Purpose { get; set; } = string.Empty;
    
    /// <summary>
    /// Loan product code
    /// </summary>
    [MaxLength(20)]
    public string ProductCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Collateral description
    /// </summary>
    [MaxLength(1000)]
    public string? CollateralDescription { get; set; }
    
    /// <summary>
    /// Collateral value
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? CollateralValue { get; set; }
    
    /// <summary>
    /// Guarantor information
    /// </summary>
    [MaxLength(1000)]
    public string? GuarantorInfo { get; set; }
    
    /// <summary>
    /// Number of missed payments
    /// </summary>
    public int MissedPayments { get; set; } = 0;
    
    /// <summary>
    /// Days past due
    /// </summary>
    public int DaysPastDue { get; set; } = 0;
    
    /// <summary>
    /// Penalty amount for overdue payments
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PenaltyAmount { get; set; } = 0;
    
    /// <summary>
    /// Group loan identifier for group lending
    /// </summary>
    public Guid? GroupLoanId { get; set; }
    
    /// <summary>
    /// Is this a group loan flag
    /// </summary>
    public bool IsGroupLoan { get; set; } = false;
    
    /// <summary>
    /// Loan classification for regulatory reporting
    /// </summary>
    public LoanClassification Classification { get; set; } = LoanClassification.Performing;
    
    /// <summary>
    /// Provision amount for non-performing loans
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal ProvisionAmount { get; set; } = 0;
    
    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "GHS";
    
    /// <summary>
    /// Approved by user ID
    /// </summary>
    [MaxLength(100)]
    public string? ApprovedBy { get; set; }
    
    /// <summary>
    /// Disbursed by user ID
    /// </summary>
    [MaxLength(100)]
    public string? DisbursedBy { get; set; }
    
    // Foreign key relationships
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// Disbursement account
    /// </summary>
    public Guid? DisbursementAccountId { get; set; }
    
    [ForeignKey(nameof(DisbursementAccountId))]
    public virtual Account? DisbursementAccount { get; set; }
    
    /// <summary>
    /// Repayment account
    /// </summary>
    public Guid? RepaymentAccountId { get; set; }
    
    [ForeignKey(nameof(RepaymentAccountId))]
    public virtual Account? RepaymentAccount { get; set; }
    
    // Navigation properties
    public virtual ICollection<LoanPayment> LoanPayments { get; set; } = new List<LoanPayment>();
    public virtual ICollection<LoanDocument> LoanDocuments { get; set; } = new List<LoanDocument>();
    public virtual ICollection<LoanApproval> LoanApprovals { get; set; } = new List<LoanApproval>();
    public virtual ICollection<Loan> GroupMembers { get; set; } = new List<Loan>(); // For group loans
}