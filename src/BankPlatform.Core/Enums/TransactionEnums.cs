namespace BankPlatform.Core.Enums;

/// <summary>
/// Transaction type enumeration
/// </summary>
public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Transfer = 3,
    Payment = 4,
    BillPayment = 5,
    AirtimeTopup = 6,
    LoanDisbursement = 7,
    LoanRepayment = 8,
    InterestCredit = 9,
    FeeDebit = 10,
    Reversal = 11,
    Refund = 12,
    MobileMoneyTransfer = 13,
    CardPayment = 14,
    OnlinePayment = 15,
    StandingOrder = 16,
    DirectDebit = 17,
    Salary = 18,
    Dividend = 19,
    Commission = 20
}

/// <summary>
/// Transaction status enumeration
/// </summary>
public enum TransactionStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Reversed = 6,
    Timeout = 7,
    UnderReview = 8,
    Approved = 9,
    Rejected = 10
}

/// <summary>
/// Transaction channel enumeration
/// </summary>
public enum TransactionChannel
{
    Branch = 1,
    ATM = 2,
    InternetBanking = 3,
    MobileBanking = 4,
    USSD = 5,
    POS = 6,
    MobileApp = 7,
    API = 8,
    BulkPayment = 9,
    DirectDebit = 10,
    StandingOrder = 11,
    AgentBanking = 12,
    CallCenter = 13,
    SMS = 14
}

/// <summary>
/// Fraud status enumeration
/// </summary>
public enum FraudStatus
{
    NotChecked = 1,
    Clean = 2,
    Suspicious = 3,
    Flagged = 4,
    Blocked = 5,
    UnderInvestigation = 6
}