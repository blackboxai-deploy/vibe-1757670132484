using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Account holder entity for joint accounts and account signatories
/// BoG compliance: Account ownership and signatory management
/// </summary>
public class AccountHolder : BaseEntity
{
    [Required]
    public Guid AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public virtual Account Account { get; set; } = null!;
    
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
    
    [Required]
    public AccountHolderType HolderType { get; set; }
    
    [Required]
    public SignatoryLevel SignatoryLevel { get; set; }
    
    /// <summary>
    /// Whether this holder is the primary account owner
    /// </summary>
    public bool IsPrimaryHolder { get; set; } = false;
    
    /// <summary>
    /// Whether this holder can perform transactions
    /// </summary>
    public bool CanTransact { get; set; } = true;
    
    /// <summary>
    /// Whether this holder can view account statements
    /// </summary>
    public bool CanViewStatements { get; set; } = true;
    
    /// <summary>
    /// Whether this holder can close the account
    /// </summary>
    public bool CanCloseAccount { get; set; } = false;
    
    /// <summary>
    /// Daily transaction limit for this holder
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DailyTransactionLimit { get; set; }
    
    /// <summary>
    /// Monthly transaction limit for this holder
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MonthlyTransactionLimit { get; set; }
    
    /// <summary>
    /// Date when holder was added to account
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when holder was removed from account
    /// </summary>
    public DateTime? RemovedAt { get; set; }
    
    /// <summary>
    /// Reason for removal
    /// </summary>
    [MaxLength(200)]
    public string? RemovalReason { get; set; }
    
    /// <summary>
    /// User who added this holder
    /// </summary>
    [MaxLength(100)]
    public string? AddedBy { get; set; }
    
    /// <summary>
    /// User who removed this holder
    /// </summary>
    [MaxLength(100)]
    public string? RemovedBy { get; set; }
}

/// <summary>
/// Account holder type enumeration
/// </summary>
public enum AccountHolderType
{
    Owner = 1,
    JointOwner = 2,
    Signatory = 3,
    Guardian = 4,
    PowerOfAttorney = 5,
    Beneficiary = 6
}

/// <summary>
/// Signatory level enumeration for transaction authorization
/// </summary>
public enum SignatoryLevel
{
    SingleSignatory = 1, // Can sign alone
    JointSignatory = 2,  // Requires another signatory
    AuthorizedSignatory = 3, // Authorized up to a limit
    ReadOnlyAccess = 4   // View only, no transaction rights
}