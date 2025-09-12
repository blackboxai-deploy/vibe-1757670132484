using System.ComponentModel.DataAnnotations;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Customer entity representing a bank customer with KYC information
/// BoG CISD compliance: Customer identification and due diligence requirements
/// </summary>
public class Customer : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? AlternatePhoneNumber { get; set; }
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    public MaritalStatus MaritalStatus { get; set; }
    
    /// <summary>
    /// Ghana Card number for NIA e-KYC integration
    /// </summary>
    [MaxLength(20)]
    public string? GhanaCardNumber { get; set; }
    
    /// <summary>
    /// National ID number from other sources
    /// </summary>
    [MaxLength(50)]
    public string? NationalIdNumber { get; set; }
    
    [MaxLength(20)]
    public string? PassportNumber { get; set; }
    
    [MaxLength(20)]
    public string? DriversLicenseNumber { get; set; }
    
    /// <summary>
    /// Customer's occupation for risk assessment
    /// </summary>
    [MaxLength(100)]
    public string? Occupation { get; set; }
    
    /// <summary>
    /// Monthly income for loan assessment
    /// </summary>
    public decimal? MonthlyIncome { get; set; }
    
    /// <summary>
    /// Source of income for AML compliance
    /// </summary>
    [MaxLength(100)]
    public string? IncomeSource { get; set; }
    
    /// <summary>
    /// KYC status for compliance tracking
    /// </summary>
    public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    
    /// <summary>
    /// KYC completion date for audit trail
    /// </summary>
    public DateTime? KycCompletedAt { get; set; }
    
    /// <summary>
    /// Risk rating for AML compliance
    /// </summary>
    public RiskRating RiskRating { get; set; } = RiskRating.Low;
    
    /// <summary>
    /// PEP (Politically Exposed Person) status
    /// </summary>
    public bool IsPoliticallyExposed { get; set; } = false;
    
    /// <summary>
    /// Unique customer number for banking operations
    /// </summary>
    [MaxLength(20)]
    public string CustomerNumber { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public virtual ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();
    public virtual ICollection<CustomerContact> EmergencyContacts { get; set; } = new List<CustomerContact>();
}