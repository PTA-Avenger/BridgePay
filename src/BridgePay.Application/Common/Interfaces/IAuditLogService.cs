namespace BridgePay.Application.Common.Interfaces;

using System;
using System.Threading.Tasks;

/// <summary>
/// Service interface for recording security and operational audit logs.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Records an audit log entry for a merchant action.
    /// </summary>
    Task LogActionAsync(
        Guid? merchantId,
        string action,
        string details,
        string ipAddress = "",
        string? correlationId = null);
}
