namespace BridgePay.Domain.Entities;

using System;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Exceptions;

/// <summary>
/// Represents a refund associated with a payment transaction.
/// </summary>
public class Refund
{
    /// <summary>
    /// Gets the unique identifier for the refund.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the transaction being refunded.
    /// </summary>
    public Guid TransactionId { get; private set; }

    /// <summary>
    /// Gets the amount to be refunded.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Gets the current status of the refund.
    /// </summary>
    public RefundStatus Status { get; private set; }

    /// <summary>
    /// Gets the reason for the refund.
    /// </summary>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the UTC date and time when the refund was requested.
    /// </summary>
    public DateTime RequestedAt { get; private set; }

    /// <summary>
    /// Gets the UTC date and time when the refund was processed.
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Gets or sets the related transaction.
    /// </summary>
    public Transaction? Transaction { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core serialization.
    /// </summary>
    private Refund() { }

    /// <summary>
    /// Creates a new Refund instance.
    /// </summary>
    public static Refund Create(Guid transactionId, decimal amount, string reason, decimal maxRefundableAmount)
    {
        if (amount <= 0)
            throw new InvalidRefundException("Refund amount must be greater than zero.");
        if (amount > maxRefundableAmount)
            throw new InvalidRefundException($"Refund amount cannot exceed {maxRefundableAmount}.");

        return new Refund
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            Amount = amount,
            Reason = reason,
            Status = RefundStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Transitions the refund status to Processing.
    /// </summary>
    public void MarkAsProcessing()
    {
        Status = RefundStatus.Processing;
    }

    /// <summary>
    /// Transitions the refund status to Completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = RefundStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the refund status to Failed.
    /// </summary>
    public void MarkAsFailed()
    {
        Status = RefundStatus.Failed;
        ProcessedAt = DateTime.UtcNow;
    }
}
