namespace BridgePay.Domain.Interfaces;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Unit of work interface for coordinating database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all changes made in the current transaction boundary to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
