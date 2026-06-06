namespace BridgePay.Application.Common.Messages;

using System;

/// <summary>
/// Event message published when a merchant requests a refund.
/// </summary>
public record RefundRequested
{
    /// <summary>Gets the refund ID.</summary>
    public Guid RefundId { get; init; }

    /// <summary>Gets the transaction ID being refunded.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the merchant ID.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the refund amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the refund reason.</summary>
    public string Reason { get; init; } = string.Empty;
}
