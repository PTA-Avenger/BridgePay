namespace BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

using System;

/// <summary>
/// Represents a refund processing request sent to a bank API.
/// </summary>
public record BankRefundRequest
{
    /// <summary>Gets the unique gateway refund ID.</summary>
    public Guid RefundId { get; init; }

    /// <summary>Gets the original bank transaction ID.</summary>
    public string BankTransactionId { get; init; } = string.Empty;

    /// <summary>Gets the amount to be refunded.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the refund reason.</summary>
    public string Reason { get; init; } = string.Empty;
}
