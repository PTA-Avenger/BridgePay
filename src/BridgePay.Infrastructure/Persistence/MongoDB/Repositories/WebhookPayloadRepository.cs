namespace BridgePay.Infrastructure.Persistence.MongoDB.Repositories;

using System;
using System.Threading.Tasks;
using BridgePay.Infrastructure.Persistence.MongoDB.Models;

/// <summary>
/// Repository for saving incoming webhook payloads from mock banks to MongoDB.
/// </summary>
public class WebhookPayloadRepository
{
    private readonly MongoDbContext _context;

    /// <summary>Initializes a new instance of WebhookPayloadRepository.</summary>
    public WebhookPayloadRepository(MongoDbContext context)
    {
        _context = context;
    }

    /// <summary>Saves the raw JSON payload of an incoming webhook to MongoDB.</summary>
    public async Task SavePayloadAsync(string bankProvider, string payload)
    {
        var document = new WebhookPayload
        {
            BankProvider = bankProvider,
            Payload = payload,
            ReceivedAt = DateTime.UtcNow
        };

        await _context.WebhookPayloads.InsertOneAsync(document);
    }
}
