namespace BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

using System;

/// <summary>
/// Represents a payment processing request sent to a bank API.
/// </summary>
public record BankTransactionRequest
{
    /// <summary>Gets the unique gateway transaction ID.</summary>
    public Guid TransactionId { get; init; }

    /// <summary>Gets the transaction amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>Gets the currency code (e.g. ZAR).</summary>
    public string Currency { get; init; } = "ZAR";

    /// <summary>Gets the payment method type.</summary>
    public string PaymentMethod { get; init; } = "CreditCard";

    /// <summary>Gets the customer reference.</summary>
    public string CustomerReference { get; init; } = string.Empty;

    /// <summary>Gets the credit/debit card number (masked/mocked).</summary>
    public string CardNumber { get; init; } = "411111******1111";
}
