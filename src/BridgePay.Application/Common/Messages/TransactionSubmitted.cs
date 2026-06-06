namespace BridgePay.Application.Common.Messages;

using System;

/// <summary>
/// Event message published when a transaction is submitted by a merchant.
/// </summary>
public record TransactionSubmitted
{
    /// <summary>Gets the transaction ID.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the merchant ID.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the transaction amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the currency.</summary>
    public string Currency { get; init; } = string.Empty;

    /// <summary>Gets the bank provider name.</summary>
    public string BankProvider { get; init; } = string.Empty;

    /// <summary>Gets the payment method.</summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>Gets the customer reference.</summary>
    public string CustomerReference { get; init; } = string.Empty;
}
