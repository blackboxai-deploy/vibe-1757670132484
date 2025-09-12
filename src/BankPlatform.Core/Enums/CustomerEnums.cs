namespace BankPlatform.Core.Enums;

/// <summary>
/// Gender enumeration
/// </summary>
public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}

/// <summary>
/// Marital status enumeration
/// </summary>
public enum MaritalStatus
{
    Single = 1,
    Married = 2,
    Divorced = 3,
    Widowed = 4,
    Separated = 5
}

/// <summary>
/// KYC status for compliance tracking
/// </summary>
public enum KycStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Rejected = 4,
    Expired = 5,
    UnderReview = 6
}

/// <summary>
/// Risk rating for AML compliance
/// </summary>
public enum RiskRating
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}