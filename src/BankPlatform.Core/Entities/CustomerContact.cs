using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Customer contact entity for emergency contacts and references
/// BoG compliance: Customer relationship and emergency contact information
/// </summary>
public class CustomerContact : BaseEntity
{
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    [Required]
    public CustomerContactType ContactType { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [MaxLength(100)]
    public string? Relationship { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(20)]
    public string? AlternatePhoneNumber { get; set; }
    
    [MaxLength(255)]
    public string? Email { get; set; }
    
    [MaxLength(200)]
    public string? AddressLine1 { get; set; }
    
    [MaxLength(200)]
    public string? AddressLine2 { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Region { get; set; }
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; } = "Ghana";
    
    /// <summary>
    /// Whether this is the primary contact for this type
    /// </summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Priority order for contact (1 = highest priority)
    /// </summary>
    public int Priority { get; set; } = 1;
    
    /// <summary>
    /// Whether this contact has been verified
    /// </summary>
    public bool IsVerified { get; set; } = false;
    
    /// <summary>
    /// Date when contact was verified
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
    
    /// <summary>
    /// User who verified this contact
    /// </summary>
    [MaxLength(100)]
    public string? VerifiedBy { get; set; }
    
    /// <summary>
    /// Verification method used
    /// </summary>
    [MaxLength(50)]
    public string? VerificationMethod { get; set; }
    
    /// <summary>
    /// Contact occupation or profession
    /// </summary>
    [MaxLength(100)]
    public string? Occupation { get; set; }
    
    /// <summary>
    /// Contact employer or workplace
    /// </summary>
    [MaxLength(200)]
    public string? Employer { get; set; }
    
    /// <summary>
    /// How long the customer has known this contact
    /// </summary>
    [MaxLength(50)]
    public string? KnownDuration { get; set; }
    
    /// <summary>
    /// Additional notes about this contact
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Whether this contact can be contacted for verification
    /// </summary>
    public bool CanContactForVerification { get; set; } = true;
    
    /// <summary>
    /// Whether this contact agreed to be a reference
    /// </summary>
    public bool ConsentGiven { get; set; } = false;
    
    /// <summary>
    /// Date when consent was given
    /// </summary>
    public DateTime? ConsentDate { get; set; }
    
    /// <summary>
    /// Contact's ID document type
    /// </summary>
    [MaxLength(50)]
    public string? IdDocumentType { get; set; }
    
    /// <summary>
    /// Contact's ID document number
    /// </summary>
    [MaxLength(100)]
    public string? IdDocumentNumber { get; set; }
    
    /// <summary>
    /// Last contact date
    /// </summary>
    public DateTime? LastContactedAt { get; set; }
    
    /// <summary>
    /// Contact status
    /// </summary>
    public ContactStatus Status { get; set; } = ContactStatus.Active;
}

/// <summary>
/// Customer contact type enumeration
/// </summary>
public enum CustomerContactType
{
    EmergencyContact = 1,
    NextOfKin = 2,
    Reference = 3,
    Guarantor = 4,
    BusinessReference = 5,
    PersonalReference = 6,
    EmploymentReference = 7,
    BankReference = 8,
    Spouse = 9,
    Parent = 10,
    Sibling = 11,
    Friend = 12,
    Colleague = 13,
    Other = 99
}

/// <summary>
/// Contact status enumeration
/// </summary>
public enum ContactStatus
{
    Active = 1,
    Inactive = 2,
    Unreachable = 3,
    Declined = 4,
    Verified = 5,
    PendingVerification = 6
}