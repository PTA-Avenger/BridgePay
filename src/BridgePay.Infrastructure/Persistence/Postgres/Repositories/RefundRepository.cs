namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// PostgreSQL repository implementation for Refund entities.
/// </summary>
public class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of RefundRepository.</summary>
    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retrieves a refund by ID.</summary>
    public async Task<Refund?> GetByIdAsync(Guid id)
    {
        return await _context.Refunds
            .Include(r => r.Transaction)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>Retrieves all refunds for a specific transaction.</summary>
    public async Task<IEnumerable<Refund>> GetByTransactionIdAsync(Guid transactionId)
    {
        return await _context.Refunds
            .Where(r => r.TransactionId == transactionId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
    }

    /// <summary>Adds a new refund.</summary>
    public async Task AddAsync(Refund refund)
    {
        await _context.Refunds.AddAsync(refund);
    }

    /// <summary>Updates an existing refund.</summary>
    public async Task UpdateAsync(Refund refund)
    {
        _context.Refunds.Update(refund);
        await Task.CompletedTask;
    }
}
