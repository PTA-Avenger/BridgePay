namespace BridgePay.Domain.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;

/// <summary>
/// Repository interface for Refund entities.
/// </summary>
public interface IRefundRepository
{
    /// <summary>
    /// Retrieves a refund by its unique identifier.
    /// </summary>
    Task<Refund?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all refunds associated with a specific transaction.
    /// </summary>
    Task<IEnumerable<Refund>> GetByTransactionIdAsync(Guid transactionId);

    /// <summary>
    /// Adds a new refund request.
    /// </summary>
    Task AddAsync(Refund refund);

    /// <summary>
    /// Updates an existing refund.
    /// </summary>
    Task UpdateAsync(Refund refund);
}
