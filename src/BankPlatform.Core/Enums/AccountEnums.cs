namespace BankPlatform.Core.Enums;

/// <summary>
/// Account type enumeration
/// </summary>
public enum AccountType
{
    Savings = 1,
    Current = 2,
    FixedDeposit = 3,
    CallDeposit = 4,
    LoanAccount = 5,
    CorporateSavings = 6,
    CorporateCurrent = 7,
    StudentAccount = 8,
    SeniorCitizenAccount = 9
}

/// <summary>
/// Account status enumeration
/// </summary>
public enum AccountStatus
{
    Active = 1,
    Inactive = 2,
    Frozen = 3,
    Closed = 4,
    Dormant = 5,
    Blocked = 6,
    UnderReview = 7
}

/// <summary>
/// Wallet type enumeration
/// </summary>
public enum WalletType
{
    Personal = 1,
    Business = 2,
    Agent = 3,
    Merchant = 4,
    Savings = 5,
    Prepaid = 6
}

/// <summary>
/// Wallet status enumeration
/// </summary>
public enum WalletStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blocked = 4,
    Pending = 5,
    Closed = 6
}