namespace BridgePay.Infrastructure.Persistence.Postgres.Repositories;

using System.Threading;
using System.Threading.Tasks;
using BridgePay.Domain.Interfaces;

/// <summary>
/// PostgreSQL implementation of the Unit of Work pattern.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of UnitOfWork.</summary>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>Persists changes to the underlying database.</summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
