namespace BridgePay.Infrastructure.Persistence.MongoDB;

using BridgePay.Infrastructure.Persistence.MongoDB.Models;
using Microsoft.Extensions.Options;
using global::MongoDB.Driver;

/// <summary>
/// MongoDB client wrapper providing access to audit logs and webhook collections.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    /// <summary>Initializes a new instance of MongoDbContext.</summary>
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    /// <summary>Gets the collection of AuditLogs.</summary>
    public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("AuditLogs");

    /// <summary>Gets the collection of WebhookPayloads.</summary>
    public IMongoCollection<WebhookPayload> WebhookPayloads => _database.GetCollection<WebhookPayload>("WebhookPayloads");
}
