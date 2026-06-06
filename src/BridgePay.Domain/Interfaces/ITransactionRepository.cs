namespace BridgePay.Domain.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Enums;

/// <summary>
/// Repository interface for Transaction entities.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Retrieves a transaction by its unique identifier.
    /// </summary>
    Task<Transaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a paginated and filtered list of transactions for a merchant.
    /// </summary>
    Task<IEnumerable<Transaction>> GetByMerchantIdAsync(
        Guid merchantId,
        int page,
        int pageSize,
        TransactionStatus? status = null,
        DateTime? from = null,
        DateTime? to = null);

    /// <summary>
    /// Adds a new transaction.
    /// </summary>
    Task AddAsync(Transaction transaction);

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    Task UpdateAsync(Transaction transaction);

    /// <summary>
    /// Counts the number of transactions matching the filters for a merchant.
    /// </summary>
    Task<int> CountByMerchantIdAsync(
        Guid merchantId,
        TransactionStatus? status = null,
        DateTime? from = null,
        DateTime? to = null);
}
