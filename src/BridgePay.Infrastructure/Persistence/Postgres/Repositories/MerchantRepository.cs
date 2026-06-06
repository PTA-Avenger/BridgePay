namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// PostgreSQL repository implementation for Merchant entities.
/// </summary>
public class MerchantRepository : IMerchantRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of MerchantRepository.</summary>
    public MerchantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Retrieves a merchant by ID.</summary>
    public async Task<Merchant?> GetByIdAsync(Guid id)
    {
        return await _context.Merchants
            .Include(m => m.ApiKeys)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    /// <summary>Retrieves a merchant by Auth User ID (linked to Supabase Auth).</summary>
    public async Task<Merchant?> GetByAuthUserIdAsync(Guid authUserId)
    {
        return await _context.Merchants
            .Include(m => m.ApiKeys)
            .FirstOrDefaultAsync(m => m.AuthUserId == authUserId);
    }

    /// <summary>Retrieves a merchant by public API Key.</summary>
    public async Task<Merchant?> GetByApiKeyAsync(string apiKey)
    {
        return await _context.Merchants
            .Include(m => m.ApiKeys)
            .FirstOrDefaultAsync(m => m.ApiKey == apiKey);
    }

    /// <summary>Adds a new merchant.</summary>
    public async Task AddAsync(Merchant merchant)
    {
        await _context.Merchants.AddAsync(merchant);
    }

    /// <summary>Updates an existing merchant.</summary>
    public async Task UpdateAsync(Merchant merchant)
    {
        _context.Merchants.Update(merchant);
        await Task.CompletedTask;
    }
}
