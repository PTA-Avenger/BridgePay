namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;
using BridgePay.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// PostgreSQL repository implementation for Transaction entities.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of TransactionRepository.</summary>
    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retrieves a transaction by ID.</summary>
    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }

    /// <summary>Retrieves transactions filtered and paginated for a merchant.</summary>
    public async Task<IEnumerable<Transaction>> GetByMerchantIdAsync(
        Guid merchantId,
        int page,
        int pageSize,
        TransactionStatus? status = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        var query = _context.Transactions
            .Where(t => t.MerchantId == merchantId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= to.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>Adds a new transaction.</summary>
    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    /// <summary>Updates an existing transaction.</summary>
    public async Task UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await Task.CompletedTask;
    }

    /// <summary>Counts transactions matching filters for a merchant.</summary>
    public async Task<int> CountByMerchantIdAsync(
        Guid merchantId,
        TransactionStatus? status = null,
        DateTime? from = null,
        DateTime? to = null)
    {
        var query = _context.Transactions
            .Where(t => t.MerchantId == merchantId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= to.Value);
        }

        return await query.CountAsync();
    }
}
