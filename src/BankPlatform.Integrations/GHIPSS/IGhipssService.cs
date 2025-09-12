using BankPlatform.Integrations.GHIPSS.Models;

namespace BankPlatform.Integrations.GHIPSS;

/// <summary>
/// Ghana Interbank Payment and Settlement Systems (GHIPSS) integration service
/// BoG compliance: Interbank transfer processing through GHIPSS
/// </summary>
public interface IGhipssService
{
    /// <summary>
    /// Process interbank transfer through GHIPSS
    /// </summary>
    Task<GhipssTransferResponse> ProcessInterBankTransferAsync(GhipssTransferRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check transaction status
    /// </summary>
    Task<GhipssStatusResponse> GetTransactionStatusAsync(string transactionReference, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validate account number with destination bank
    /// </summary>
    Task<GhipssAccountValidationResponse> ValidateAccountAsync(string accountNumber, string bankCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get list of participating banks
    /// </summary>
    Task<GhipssBankListResponse> GetParticipatingBanksAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process ACH (Automated Clearing House) transactions
    /// </summary>
    Task<GhipssAchResponse> ProcessAchTransactionAsync(GhipssAchRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Submit bulk payment file
    /// </summary>
    Task<GhipssBulkPaymentResponse> SubmitBulkPaymentAsync(GhipssBulkPaymentRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get settlement report
    /// </summary>
    Task<GhipssSettlementResponse> GetSettlementReportAsync(DateTime settlementDate, CancellationToken cancellationToken = default);
}