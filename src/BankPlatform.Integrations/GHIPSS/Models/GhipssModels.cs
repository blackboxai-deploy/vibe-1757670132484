using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankPlatform.Integrations.GHIPSS.Models;

/// <summary>
/// GHIPSS transfer request model
/// </summary>
public class GhipssTransferRequest
{
    [Required]
    [JsonPropertyName("transactionReference")]
    public string TransactionReference { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("debitAccount")]
    public string DebitAccount { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("debitBankCode")]
    public string DebitBankCode { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("creditAccount")]
    public string CreditAccount { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("creditBankCode")]
    public string CreditBankCode { get; set; } = string.Empty;
    
    [Required]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [Required]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "GHS";
    
    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
    
    [JsonPropertyName("beneficiaryName")]
    public string? BeneficiaryName { get; set; }
    
    [JsonPropertyName("senderName")]
    public string? SenderName { get; set; }
    
    [JsonPropertyName("valueDate")]
    public DateTime ValueDate { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "NORMAL"; // NORMAL, HIGH, URGENT
}

/// <summary>
/// GHIPSS transfer response model
/// </summary>
public class GhipssTransferResponse
{
    [JsonPropertyName("transactionReference")]
    public string TransactionReference { get; set; } = string.Empty;
    
    [JsonPropertyName("ghipssReference")]
    public string GhipssReference { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }
    
    [JsonPropertyName("processedAt")]
    public DateTime ProcessedAt { get; set; }
    
    [JsonPropertyName("settlementDate")]
    public DateTime? SettlementDate { get; set; }
    
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }
    
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// GHIPSS status response model
/// </summary>
public class GhipssStatusResponse
{
    [JsonPropertyName("transactionReference")]
    public string TransactionReference { get; set; } = string.Empty;
    
    [JsonPropertyName("ghipssReference")]
    public string GhipssReference { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;
    
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }
    
    [JsonPropertyName("settlementStatus")]
    public string? SettlementStatus { get; set; }
    
    [JsonPropertyName("failureReason")]
    public string? FailureReason { get; set; }
}

/// <summary>
/// GHIPSS account validation response
/// </summary>
public class GhipssAccountValidationResponse
{
    [JsonPropertyName("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("bankCode")]
    public string BankCode { get; set; } = string.Empty;
    
    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }
    
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>
/// GHIPSS bank list response
/// </summary>
public class GhipssBankListResponse
{
    [JsonPropertyName("banks")]
    public List<GhipssBankInfo> Banks { get; set; } = new();
    
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// GHIPSS bank information
/// </summary>
public class GhipssBankInfo
{
    [JsonPropertyName("bankCode")]
    public string BankCode { get; set; } = string.Empty;
    
    [JsonPropertyName("bankName")]
    public string BankName { get; set; } = string.Empty;
    
    [JsonPropertyName("sortCode")]
    public string? SortCode { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("services")]
    public List<string> Services { get; set; } = new();
}

/// <summary>
/// GHIPSS ACH request model
/// </summary>
public class GhipssAchRequest
{
    [Required]
    [JsonPropertyName("batchReference")]
    public string BatchReference { get; set; } = string.Empty;
    
    [JsonPropertyName("transactions")]
    public List<GhipssAchTransaction> Transactions { get; set; } = new();
    
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("transactionCount")]
    public int TransactionCount { get; set; }
    
    [JsonPropertyName("processingDate")]
    public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// GHIPSS ACH transaction
/// </summary>
public class GhipssAchTransaction
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    
    [JsonPropertyName("debitAccount")]
    public string DebitAccount { get; set; } = string.Empty;
    
    [JsonPropertyName("creditAccount")]
    public string CreditAccount { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("narration")]
    public string? Narration { get; set; }
}

/// <summary>
/// GHIPSS ACH response model
/// </summary>
public class GhipssAchResponse
{
    [JsonPropertyName("batchReference")]
    public string BatchReference { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("processedCount")]
    public int ProcessedCount { get; set; }
    
    [JsonPropertyName("failedCount")]
    public int FailedCount { get; set; }
    
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("results")]
    public List<GhipssAchResult> Results { get; set; } = new();
}

/// <summary>
/// GHIPSS ACH result
/// </summary>
public class GhipssAchResult
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("ghipssReference")]
    public string? GhipssReference { get; set; }
    
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }
    
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// GHIPSS bulk payment request
/// </summary>
public class GhipssBulkPaymentRequest
{
    [JsonPropertyName("fileReference")]
    public string FileReference { get; set; } = string.Empty;
    
    [JsonPropertyName("fileContent")]
    public byte[] FileContent { get; set; } = Array.Empty<byte>();
    
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;
    
    [JsonPropertyName("fileFormat")]
    public string FileFormat { get; set; } = "CSV"; // CSV, XML, MT940
    
    [JsonPropertyName("processingDate")]
    public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// GHIPSS bulk payment response
/// </summary>
public class GhipssBulkPaymentResponse
{
    [JsonPropertyName("fileReference")]
    public string FileReference { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("validationStatus")]
    public string ValidationStatus { get; set; } = string.Empty;
    
    [JsonPropertyName("transactionCount")]
    public int TransactionCount { get; set; }
    
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }
    
    [JsonPropertyName("validationErrors")]
    public List<string> ValidationErrors { get; set; } = new();
}

/// <summary>
/// GHIPSS settlement response
/// </summary>
public class GhipssSettlementResponse
{
    [JsonPropertyName("settlementDate")]
    public DateTime SettlementDate { get; set; }
    
    [JsonPropertyName("totalCredit")]
    public decimal TotalCredit { get; set; }
    
    [JsonPropertyName("totalDebit")]
    public decimal TotalDebit { get; set; }
    
    [JsonPropertyName("netPosition")]
    public decimal NetPosition { get; set; }
    
    [JsonPropertyName("transactionCount")]
    public int TransactionCount { get; set; }
    
    [JsonPropertyName("settlements")]
    public List<GhipssSettlementItem> Settlements { get; set; } = new();
}

/// <summary>
/// GHIPSS settlement item
/// </summary>
public class GhipssSettlementItem
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("counterparty")]
    public string Counterparty { get; set; } = string.Empty;
    
    [JsonPropertyName("settledAt")]
    public DateTime SettledAt { get; set; }
}