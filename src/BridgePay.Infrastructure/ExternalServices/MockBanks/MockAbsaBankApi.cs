namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System;
using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Mock implementation for Absa Bank API.
/// </summary>
public class MockAbsaBankApi : IBankApi
{
    /// <summary>Submits a payment with 150-600ms delay and ~18% simulated failure rate.</summary>
    public async Task<BankTransactionResponse> SubmitPaymentAsync(BankTransactionRequest request)
    {
        // Simulate network latency
        var delay = Random.Shared.Next(150, 600);
        await Task.Delay(delay);

        // Simulate failure (~18%)
        if (Random.Shared.NextDouble() < 0.18)
        {
            var errors = new[]
            {
                ("ABSA_SUSPECTED_FRAUD", "The transaction was flagged for suspected fraud."),
                ("ABSA_INSUFFICIENT_FUNDS", "Absa account has insufficient balance."),
                ("ABSA_SYSTEM_TIMEOUT", "Absa host timed out during authorization.")
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
            BankTransactionId = $"ABS-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }

    /// <summary>Submits a refund with 100-500ms delay and ~6% simulated failure rate.</summary>
    public async Task<BankRefundResponse> SubmitRefundAsync(BankRefundRequest request)
    {
        var delay = Random.Shared.Next(100, 500);
        await Task.Delay(delay);

        if (Random.Shared.NextDouble() < 0.06)
        {
            return new BankRefundResponse
            {
                Success = false,
                ErrorCode = "ABSA_REFUND_DECLINED",
                ErrorMessage = "Absa rejected the refund request."
            };
        }

        return new BankRefundResponse
        {
            Success = true,
            BankTransactionId = $"REF-ABS-{Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant()}"
        };
    }
}
