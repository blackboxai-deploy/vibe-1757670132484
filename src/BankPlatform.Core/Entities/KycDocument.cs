using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// KYC document entity for customer verification documents
/// BoG compliance: Document requirements for customer identification
/// </summary>
public class KycDocument : BaseEntity
{
    [Required]
    public DocumentType DocumentType { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string DocumentNumber { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string DocumentPath { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? DocumentName { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public DateTime? IssueDate { get; set; }
    
    [MaxLength(100)]
    public string? IssuingAuthority { get; set; }
    
    /// <summary>
    /// Document verification status
    /// </summary>
    public DocumentVerificationStatus VerificationStatus { get; set; } = DocumentVerificationStatus.Pending;
    
    /// <summary>
    /// Verification date
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
    
    /// <summary>
    /// Verification remarks
    /// </summary>
    [MaxLength(500)]
    public string? VerificationRemarks { get; set; }
    
    /// <summary>
    /// Verified by user ID
    /// </summary>
    [MaxLength(100)]
    public string? VerifiedBy { get; set; }
    
    /// <summary>
    /// Document file size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Document MIME type
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; set; }
    
    /// <summary>
    /// Document hash for integrity verification
    /// </summary>
    [MaxLength(500)]
    public string? DocumentHash { get; set; }
    
    /// <summary>
    /// Encrypted flag for sensitive documents
    /// </summary>
    public bool IsEncrypted { get; set; } = false;
    
    // Foreign key relationship
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
}

/// <summary>
/// Document type enumeration for KYC
/// </summary>
public enum DocumentType
{
    GhanaCard = 1,
    Passport = 2,
    DriversLicense = 3,
    VotersId = 4,
    BirthCertificate = 5,
    UtilityBill = 6,
    BankStatement = 7,
    ProofOfIncome = 8,
    BusinessRegistration = 9,
    TaxCertificate = 10,
    ProofOfAddress = 11,
    EmploymentLetter = 12,
    StudentId = 13,
    MarriageCertificate = 14,
    Other = 99
}

/// <summary>
/// Document verification status enumeration
/// </summary>
public enum DocumentVerificationStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3,
    Expired = 4,
    UnderReview = 5,
    RequiresAdditionalInfo = 6
}