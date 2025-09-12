using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Double-entry bookkeeping transaction entries
/// BoG compliance: Proper accounting records for all transactions
/// </summary>
public class TransactionEntry : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Required]
    public EntryType EntryType { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "GHS";
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime PostingDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime ValueDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Balance after this entry
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAfter { get; set; }
    
    /// <summary>
    /// Entry sequence number for ordering
    /// </summary>
    public int SequenceNumber { get; set; }
    
    // Foreign key relationship
    [Required]
    public Guid TransactionId { get; set; }
    
    [ForeignKey(nameof(TransactionId))]
    public virtual Transaction Transaction { get; set; } = null!;
    
    public Guid? AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public virtual Account? Account { get; set; }
}

/// <summary>
/// Entry type enumeration for double-entry bookkeeping
/// </summary>
public enum EntryType
{
    Debit = 1,
    Credit = 2
}