namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;

/// <summary>
/// Repository for saving incoming webhook payloads from mock banks to PostgreSQL.
/// </summary>
public class WebhookPayloadRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of WebhookPayloadRepository.</summary>
    public WebhookPayloadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Saves the raw JSON payload of an incoming webhook to PostgreSQL.</summary>
    public async Task SavePayloadAsync(string bankProvider, string payload)
    {
        var document = WebhookPayload.Create(bankProvider, payload);
        await _context.WebhookPayloads.AddAsync(document);
        await _context.SaveChangesAsync();
    }
}
