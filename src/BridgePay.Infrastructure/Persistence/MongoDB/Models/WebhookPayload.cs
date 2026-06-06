namespace BridgePay.Infrastructure.Persistence.MongoDB.Models;

using System;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a raw webhook payload received from external bank APIs.
/// </summary>
public class WebhookPayload
{
    /// <summary>Gets or sets the MongoDB document ID.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>Gets or sets the name of the bank provider.</summary>
    public string BankProvider { get; set; } = string.Empty;

    /// <summary>Gets or sets the raw JSON payload contents.</summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC date and time when received.</summary>
    public DateTime ReceivedAt { get; set; }
}
