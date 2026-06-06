namespace BridgePay.Application.Common.Messages;

using System;

/// <summary>
/// Event message published when a transaction is successfully processed and completed by the bank.
/// </summary>
public record TransactionCompleted
{
    /// <summary>Gets the transaction ID.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the merchant ID.</summary>
    public Guid MerchantId { get; init; }

    /// <summary>Gets the bank-provided transaction ID.</summary>
    public string BankTransactionId { get; init; } = string.Empty;

    /// <summary>Gets the transaction amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the fee charged.</summary>
    public decimal FeeAmount { get; init; }

    /// <summary>Gets the net amount settled.</summary>
    public decimal NetAmount { get; init; }
}
