namespace BridgePay.Infrastructure.Persistence.MongoDB.Models;

using System;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// Represents a security/operational audit log document stored in MongoDB.
/// </summary>
public class AuditLog
{
    /// <summary>Gets or sets the MongoDB document ID.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>Gets or sets the associated merchant ID, if any.</summary>
    public Guid? MerchantId { get; set; }

    /// <summary>Gets or sets the performed action name.</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Gets or sets details/metadata of the action.</summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>Gets or sets the request source IP address.</summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the correlation identifier for tracking request flows.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Gets or sets the UTC timestamp when the action occurred.</summary>
    public DateTime Timestamp { get; set; }
}
