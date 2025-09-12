using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankPlatform.Core.Enums;

namespace BankPlatform.Core.Entities;

/// <summary>
/// Address entity for customer addresses
/// BoG compliance: Customer address verification for KYC
/// </summary>
public class Address : BaseEntity
{
    [Required]
    public AddressType AddressType { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? AddressLine2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "Ghana";
    
    /// <summary>
    /// Primary address flag
    /// </summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Address verification status
    /// </summary>
    public bool IsVerified { get; set; } = false;
    
    /// <summary>
    /// Date when address was verified
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
    
    /// <summary>
    /// GPS coordinates for address verification
    /// </summary>
    [MaxLength(50)]
    public string? Latitude { get; set; }
    
    [MaxLength(50)]
    public string? Longitude { get; set; }
    
    /// <summary>
    /// Landmark description
    /// </summary>
    [MaxLength(200)]
    public string? Landmark { get; set; }
    
    // Foreign key relationship
    [Required]
    public Guid CustomerId { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer Customer { get; set; } = null!;
}

/// <summary>
/// Address type enumeration
/// </summary>
public enum AddressType
{
    Residential = 1,
    Business = 2,
    Postal = 3,
    Temporary = 4
}