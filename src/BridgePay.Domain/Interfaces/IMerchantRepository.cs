namespace BridgePay.Domain.Interfaces;

using System;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;

/// <summary>
/// Repository interface for Merchant entities.
/// </summary>
public interface IMerchantRepository
{
    /// <summary>
    /// Retrieves a merchant by their unique identifier.
    /// </summary>
    Task<Merchant?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a merchant by their Auth User ID.
    /// </summary>
    Task<Merchant?> GetByAuthUserIdAsync(Guid authUserId);

    /// <summary>
    /// Retrieves a merchant by their public API key.
    /// </summary>
    Task<Merchant?> GetByApiKeyAsync(string apiKey);

    /// <summary>
    /// Adds a new merchant.
    /// </summary>
    Task AddAsync(Merchant merchant);

    /// <summary>
    /// Updates an existing merchant.
    /// </summary>
    Task UpdateAsync(Merchant merchant);
}
