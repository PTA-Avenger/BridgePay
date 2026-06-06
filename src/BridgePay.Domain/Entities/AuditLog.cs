namespace BridgePay.Domain.Entities;

using System;

/// <summary>
/// Represents an audit log entry tracking API actions and events.
/// </summary>
public class AuditLog
{
    /// <summary>Gets the unique identifier for the audit log.</summary>
    public Guid Id { get; private set; }

    /// <summary>Gets the merchant identifier associated with the action, if any.</summary>
    public Guid? MerchantId { get; private set; }

    /// <summary>Gets the action name (e.g. SubmitTransaction, RequestRefund).</summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>Gets detailed information about the action/event.</summary>
    public string Details { get; private set; } = string.Empty;

    /// <summary>Gets the IP address from which the request originated.</summary>
    public string IpAddress { get; private set; } = string.Empty;

    /// <summary>Gets the correlation ID linking related requests and async tasks.</summary>
    public string? CorrelationId { get; private set; }

    /// <summary>Gets the UTC date and time when the event occurred.</summary>
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    /// <summary>Creates a new AuditLog entry.</summary>
    public static AuditLog Create(Guid? merchantId, string action, string details, string ipAddress, string? correlationId)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Action = action,
            Details = details,
            IpAddress = ipAddress,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };
    }
}
