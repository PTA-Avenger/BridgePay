namespace BridgePay.Application.Common.DTOs;

using System;

/// <summary>
/// Data transfer object representing a transaction.
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the transaction.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the merchant.
    /// </summary>
    public Guid MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the transaction amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the transaction.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank provider.
    /// </summary>
    public string BankProvider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank transaction ID.
    /// </summary>
    public string? BankTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the transaction fee amount.
    /// </summary>
    public decimal FeeAmount { get; set; }

    /// <summary>
    /// Gets or sets the net amount settled to the merchant.
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer reference.
    /// </summary>
    public string CustomerReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the failure reason, if any.
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the transaction was processed.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
}
