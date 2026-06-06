namespace BridgePay.Infrastructure.Persistence.MongoDB.Repositories;

using System;
using System.Threading.Tasks;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Infrastructure.Persistence.MongoDB.Models;

/// <summary>
/// MongoDB implementation of the audit logging service.
/// </summary>
public class AuditLogRepository : IAuditLogService
{
    private readonly MongoDbContext _context;

    /// <summary>Initializes a new instance of AuditLogRepository.</summary>
    public AuditLogRepository(MongoDbContext context)
    {
        _context = context;
    }

    /// <summary>Inserts a new audit log record into MongoDB asynchronously.</summary>
    public async Task LogActionAsync(
        Guid? merchantId,
        string action,
        string details,
        string ipAddress = "",
        string? correlationId = null)
    {
        var log = new AuditLog
        {
            MerchantId = merchantId,
            Action = action,
            Details = details,
            IpAddress = ipAddress,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };

        await _context.AuditLogs.InsertOneAsync(log);
    }
}
