using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Loan document entity for loan-related documentation
/// BoG compliance: Loan documentation and record keeping
/// </summary>
public class LoanDocument : BaseEntity
{
    [Required]
    public Guid LoanId { get; set; }
    
    [ForeignKey(nameof(LoanId))]
    public virtual Loan Loan { get; set; } = null!;
    
    [Required]
    public LoanDocumentType DocumentType { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string DocumentName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string DocumentPath { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? DocumentNumber { get; set; }
    
    [MaxLength(100)]
    public string? MimeType { get; set; }
    
    public long FileSize { get; set; }
    
    [MaxLength(500)]
    public string? DocumentHash { get; set; }
    
    [Required]
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    
    /// <summary>
    /// Whether this document is required for loan processing
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// Document upload date
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// User who uploaded the document
    /// </summary>
    [MaxLength(100)]
    public string? UploadedBy { get; set; }
    
    /// <summary>
    /// Document review/verification status
    /// </summary>
    public DocumentVerificationStatus VerificationStatus { get; set; } = DocumentVerificationStatus.Pending;
    
    /// <summary>
    /// Date when document was reviewed
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    
    /// <summary>
    /// User who reviewed the document
    /// </summary>
    [MaxLength(100)]
    public string? ReviewedBy { get; set; }
    
    /// <summary>
    /// Review comments
    /// </summary>
    [MaxLength(1000)]
    public string? ReviewComments { get; set; }
    
    /// <summary>
    /// Document expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    
    /// <summary>
    /// Whether document is expired
    /// </summary>
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    
    /// <summary>
    /// Document version number
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Previous version document ID
    /// </summary>
    public Guid? PreviousVersionId { get; set; }
    
    /// <summary>
    /// Whether this document contains sensitive information
    /// </summary>
    public bool IsSensitive { get; set; } = true;
    
    /// <summary>
    /// Whether document is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; } = false;
    
    /// <summary>
    /// Document tags for categorization
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }
    
    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    [MaxLength(1000)]
    public string? Metadata { get; set; }
}

/// <summary>
/// Loan document type enumeration
/// </summary>
public enum LoanDocumentType
{
    LoanApplication = 1,
    IncomeVerification = 2,
    ProofOfEmployment = 3,
    BankStatements = 4,
    CollateralDocuments = 5,
    GuarantorDocuments = 6,
    LoanAgreement = 7,
    SecurityDocuments = 8,
    IdentificationDocuments = 9,
    BusinessRegistration = 10,
    FinancialStatements = 11,
    TaxReturns = 12,
    PropertyValuation = 13,
    InsuranceDocuments = 14,
    DirectorResolution = 15,
    BoardResolution = 16,
    PowerOfAttorney = 17,
    LoanSchedule = 18,
    PromissoryNote = 19,
    OtherSupporting = 99
}

/// <summary>
/// Document status enumeration
/// </summary>
public enum DocumentStatus
{
    Pending = 1,
    Uploaded = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Expired = 6,
    Archived = 7
}