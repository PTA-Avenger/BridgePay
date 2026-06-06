namespace BridgePay.Domain.Entities;

using System;

/// <summary>
/// Represents a raw webhook payload received from external mock banks.
/// </summary>
public class WebhookPayload
{
    /// <summary>Gets the unique identifier for the webhook payload record.</summary>
    public Guid Id { get; private set; }

    /// <summary>Gets the bank provider that sent the webhook.</summary>
    public string BankProvider { get; private set; } = string.Empty;

    /// <summary>Gets the raw JSON payload body.</summary>
    public string Payload { get; private set; } = string.Empty;

    /// <summary>Gets the UTC date and time when the webhook was received.</summary>
    public DateTime ReceivedAt { get; private set; }

    private WebhookPayload() { }

    /// <summary>Creates a new WebhookPayload entry.</summary>
    public static WebhookPayload Create(string bankProvider, string payload)
    {
        return new WebhookPayload
        {
            Id = Guid.NewGuid(),
            BankProvider = bankProvider,
            Payload = payload,
            ReceivedAt = DateTime.UtcNow
        };
    }
}
