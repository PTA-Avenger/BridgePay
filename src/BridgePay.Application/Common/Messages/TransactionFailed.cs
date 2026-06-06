namespace BridgePay.Application.Common.Messages;

using System;

/// <summary>
/// Event message published when a transaction processing fails.
/// </summary>
public record TransactionFailed
{
    /// <summary>Gets the transaction ID.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the merchant ID.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the reason for the failure.</summary>
    public string FailureReason { get; init; } = string.Empty;
}
