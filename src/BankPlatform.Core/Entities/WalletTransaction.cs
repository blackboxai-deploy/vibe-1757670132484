using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Wallet-specific transaction entity for digital payments
/// </summary>
public class WalletTransaction : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string TransactionReference { get; set; } = string.Empty;
    
    [Required]
    public WalletTransactionType TransactionType { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Fee { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [Required]
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [MaxLength(20)]
    public string? RecipientWallet { get; set; }
    
    [MaxLength(20)]
    public string? RecipientMobile { get; set; }
    
    /// <summary>
    /// Mobile money transaction ID
    /// </summary>
    [MaxLength(100)]
    public string? ExternalTransactionId { get; set; }
    
    /// <summary>
    /// Provider-specific reference
    /// </summary>
    [MaxLength(100)]
    public string? ProviderReference { get; set; }
    
    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    // Foreign key relationship
    [Required]
    public Guid WalletId { get; set; }
    
    [ForeignKey(nameof(WalletId))]
    public virtual Wallet Wallet { get; set; } = null!;
}

/// <summary>
/// Wallet transaction type enumeration
/// </summary>
public enum WalletTransactionType
{
    CashIn = 1,
    CashOut = 2,
    P2P = 3, // Person to Person
    B2P = 4, // Business to Person
    P2B = 5, // Person to Business
    BillPayment = 6,
    AirtimeTopup = 7,
    Transfer = 8
}