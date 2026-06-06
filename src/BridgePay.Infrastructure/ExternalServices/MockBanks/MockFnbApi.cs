namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System;
using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Mock implementation for First National Bank (FNB) API.
/// </summary>
public class MockFnbApi : IBankApi
{
    /// <summary>Submits a payment with 200-800ms delay and ~12% simulated failure rate.</summary>
    public async Task<BankTransactionResponse> SubmitPaymentAsync(BankTransactionRequest request)
    {
        // Simulate network latency
        var delay = Random.Shared.Next(200, 800);
        await Task.Delay(delay);

        // Simulate failure (~12%)
        if (Random.Shared.NextDouble() < 0.12)
        {
            var errors = new[]
            {
                ("FNB_INSUFFICIENT_FUNDS", "FNB account does not have enough funds."),
                ("FNB_LIMIT_EXCEEDED", "The customer's daily limit has been exceeded."),
                ("FNB_RESTRICTED_CARD", "The card is restricted or flagged.")
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
            BankTransactionId = $"FNB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }

    /// <summary>Submits a refund with 150-400ms delay and ~4% simulated failure rate.</summary>
    public async Task<BankRefundResponse> SubmitRefundAsync(BankRefundRequest request)
    {
        var delay = Random.Shared.Next(150, 400);
        await Task.Delay(delay);

        if (Random.Shared.NextDouble() < 0.04)
        {
            return new BankRefundResponse
            {
                Success = false,
                ErrorCode = "FNB_REFUND_FAILED",
                ErrorMessage = "FNB bank rejected the refund."
            };
        }

        return new BankRefundResponse
        {
            Success = true,
            BankTransactionId = $"REF-FNB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }
}
