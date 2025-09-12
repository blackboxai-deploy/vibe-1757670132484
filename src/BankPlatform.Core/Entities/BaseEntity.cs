using System.ComponentModel.DataAnnotations;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Base entity class providing common properties for all domain entities
/// </summary>
public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public string CreatedBy { get; set; } = string.Empty;
    
    public string? UpdatedBy { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Soft delete flag - BoG compliance requires audit trail retention
    /// </summary>
    public bool IsDeleted { get; set; } = false;
    
    public DateTime? DeletedAt { get; set; }
    
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}