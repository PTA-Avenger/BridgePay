namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Defines the interface for communicating with core financial bank APIs (Implementation in Bridge pattern).
/// </summary>
public interface IBankApi
{
    /// <summary>
    /// Submits a payment transaction to the bank for authorization and clearing.
    /// </summary>
    Task<BankTransactionResponse> SubmitPaymentAsync(BankTransactionRequest request);

    /// <summary>
    /// Submits a refund request to the bank.
    /// </summary>
    Task<BankRefundResponse> SubmitRefundAsync(BankRefundRequest request);
}
