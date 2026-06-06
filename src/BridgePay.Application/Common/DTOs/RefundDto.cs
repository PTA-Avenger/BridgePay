namespace BridgePay.Application.Common.DTOs;

using System;

/// <summary>
/// Data transfer object representing a refund.
/// </summary>
public class RefundDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the refund.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the associated transaction identifier.
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// Gets or sets the refund amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the status of the refund.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason for the refund.
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the refund was requested.
    /// </summary>
    public DateTime RequestedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the refund was processed.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
}
