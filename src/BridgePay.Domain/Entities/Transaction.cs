namespace BridgePay.Domain.Entities;

using System;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;

/// <summary>
/// Represents a payment transaction in the BridgePay system.
/// </summary>
public class Transaction
{
    /// <summary>
    /// Gets the unique identifier for the transaction.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the merchant who initiated the transaction.
    /// </summary>
    public Guid MerchantId { get; private set; }

    /// <summary>
    /// Gets the transaction amount.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Gets the ISO currency code for the transaction.
    /// </summary>
    public string Currency { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current status of the transaction.
    /// </summary>
    public TransactionStatus Status { get; private set; }

    /// <summary>
    /// Gets the bank provider used for this transaction.
    /// </summary>
    public BankProvider BankProvider { get; private set; }

    /// <summary>
    /// Gets the transaction ID returned by the bank, if processed.
    /// </summary>
    public string? BankTransactionId { get; private set; }

    /// <summary>
    /// Gets the fee amount charged for this transaction.
    /// </summary>
    public decimal FeeAmount { get; private set; }

    /// <summary>
    /// Gets the net amount (Amount - FeeAmount) settled to the merchant.
    /// </summary>
    public decimal NetAmount { get; private set; }

    /// <summary>
    /// Gets the payment method used.
    /// </summary>
    public string PaymentMethod { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the customer reference for this transaction.
    /// </summary>
    public string CustomerReference { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the reason for failure if the transaction failed.
    /// </summary>
    public string? FailureReason { get; private set; }

    /// <summary>
    /// Gets the UTC date and time when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the UTC date and time when the transaction was processed.
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core serialization.
    /// </summary>
    private Transaction() { }

    /// <summary>
    /// Creates a new Transaction instance.
    /// </summary>
    public static Transaction Create(Guid merchantId, decimal amount, string currency, BankProvider bankProvider, string paymentMethod, string customerReference)
    {
        if (amount <= 0)
            throw new InvalidTransactionException("Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new InvalidTransactionException("Currency is required.");

        return new Transaction
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Amount = amount,
            Currency = currency,
            BankProvider = bankProvider,
            PaymentMethod = paymentMethod,
            CustomerReference = customerReference,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Calculates the gateway/bank processing fee and the resulting net amount.
    /// </summary>
    public void CalculateFee(decimal feePercentage, decimal flatFee)
    {
        FeeAmount = Math.Round(Amount * feePercentage / 100m + flatFee, 4);
        NetAmount = Amount - FeeAmount;
    }

    /// <summary>
    /// Transitions the transaction status to Processing.
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = TransactionStatus.Processing;
    }

    /// <summary>
    /// Transitions the transaction status to Completed.
    /// </summary>
    public void MarkAsCompleted(string bankTransactionId)
    {
        BankTransactionId = bankTransactionId;
        Status = TransactionStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the transaction status to Failed.
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        FailureReason = reason;
        Status = TransactionStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the transaction status to Refunded.
    /// </summary>
    public void MarkAsRefunded()
    {
        Status = TransactionStatus.Refunded;
    }

    /// <summary>
    /// Transitions the transaction status to PartiallyRefunded.
    /// </summary>
    public void MarkAsPartiallyRefunded()
    {
        Status = TransactionStatus.PartiallyRefunded;
    }
}
