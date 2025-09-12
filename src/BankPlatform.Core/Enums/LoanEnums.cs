namespace BankPlatform.Core.Enums;

/// <summary>
/// Loan type enumeration
/// </summary>
public enum LoanType
{
    PersonalLoan = 1,
    BusinessLoan = 2,
    MortgageLoan = 3,
    VehicleLoan = 4,
    EducationLoan = 5,
    AgriculturalLoan = 6,
    MicrocreditLoan = 7,
    GroupLoan = 8,
    SalaryAdvance = 9,
    OverdraftFacility = 10,
    SMELoan = 11,
    ConsumerLoan = 12
}

/// <summary>
/// Loan status enumeration
/// </summary>
public enum LoanStatus
{
    Applied = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Disbursed = 5,
    Active = 6,
    Overdue = 7,
    DefaultValue = 8,
    FullyPaid = 9,
    WrittenOff = 10,
    Restructured = 11,
    Cancelled = 12
}

/// <summary>
/// Loan classification for regulatory reporting
/// </summary>
public enum LoanClassification
{
    Performing = 1,
    SpecialMention = 2,
    Substandard = 3,
    Doubtful = 4,
    Loss = 5
}