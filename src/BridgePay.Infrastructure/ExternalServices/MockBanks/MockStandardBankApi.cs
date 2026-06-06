namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System;
using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Mock implementation for Standard Bank of South Africa API.
/// </summary>
public class MockStandardBankApi : IBankApi
{
    /// <summary>Submits a payment with 100-500ms delay and ~15% simulated failure rate.</summary>
    public async Task<BankTransactionResponse> SubmitPaymentAsync(BankTransactionRequest request)
    {
        // Simulate network latency
        var delay = Random.Shared.Next(100, 500);
        await Task.Delay(delay);

        // Simulate failure (~15%)
        if (Random.Shared.NextDouble() < 0.15)
        {
            var errors = new[]
            {
                ("INSUFFICIENT_FUNDS", "The account has insufficient funds to cover the transaction."),
                ("CARD_EXPIRED", "The customer's card has expired."),
                ("DECLINED", "The transaction was declined by the card issuer.")
            };

            var selectedError = errors[Random.Shared.Next(errors.Length)];

            return new BankTransactionResponse
            {
                Success = false,
                ErrorCode = selectedError.Item1,
                ErrorMessage = selectedError.Item2
            };
        }

        // Return success
        return new BankTransactionResponse
        {
            Success = true,
            BankTransactionId = $"STD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }

    /// <summary>Submits a refund with 100-300ms delay and ~5% simulated failure rate.</summary>
    public async Task<BankRefundResponse> SubmitRefundAsync(BankRefundRequest request)
    {
        var delay = Random.Shared.Next(100, 300);
        await Task.Delay(delay);

        if (Random.Shared.NextDouble() < 0.05)
        {
            return new BankRefundResponse
            {
                Success = false,
                ErrorCode = "REFUND_REJECTED",
                ErrorMessage = "Standard Bank rejected the refund request."
            };
        }

        return new BankRefundResponse
        {
            Success = true,
            BankTransactionId = $"REF-STD-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }
}
