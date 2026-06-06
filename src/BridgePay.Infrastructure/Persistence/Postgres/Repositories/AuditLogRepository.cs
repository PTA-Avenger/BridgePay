namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System;
using System.Threading.Tasks;
using BridgePay.Application.Common.Interfaces;
using BridgePay.Domain.Entities;

/// <summary>
/// PostgreSQL implementation of the audit logging service.
/// </summary>
public class AuditLogRepository : IAuditLogService
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of AuditLogRepository.</summary>
    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Inserts a new audit log record into PostgreSQL asynchronously.</summary>
    public async Task LogActionAsync(
        Guid? merchantId,
        string action,
        string details,
        string ipAddress = "",
        string? correlationId = null)
    {
        var log = AuditLog.Create(merchantId, action, details, ipAddress, correlationId);
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
