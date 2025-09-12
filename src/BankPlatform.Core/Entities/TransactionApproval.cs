using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Transaction approval entity for maker-checker workflow
/// BoG compliance: Approval workflow for high-value transactions
/// </summary>
public class TransactionApproval : BaseEntity
{
    [Required]
    public Guid TransactionId { get; set; }
    
    [ForeignKey(nameof(TransactionId))]
    public virtual Transaction Transaction { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string ApproverId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string ApproverName { get; set; } = string.Empty;
    
    [Required]
    public ApprovalStatus Status { get; set; }
    
    [Required]
    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(1000)]
    public string? Comments { get; set; }
    
    /// <summary>
    /// Reason for rejection if applicable
    /// </summary>
    [MaxLength(500)]
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Approval level (1st level, 2nd level, etc.)
    /// </summary>
    public int ApprovalLevel { get; set; } = 1;
    
    /// <summary>
    /// Whether this approval is required or optional
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
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
}

/// <summary>
/// Approval status enumeration
/// </summary>
public enum ApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Expired = 4
}