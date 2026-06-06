namespace BridgePay.Infrastructure.Persistence.Postgres;

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core database context for the BridgePay system using PostgreSQL.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>Initializes a new instance of ApplicationDbContext.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the Merchants DbSet.</summary>
    public DbSet<Merchant> Merchants => Set<Merchant>();

    /// <summary>Gets or sets the Transactions DbSet.</summary>
    public DbSet<Transaction> Transactions => Set<Transaction>();

    /// <summary>Gets or sets the Refunds DbSet.</summary>
    public DbSet<Refund> Refunds => Set<Refund>();

    /// <summary>Gets or sets the ApiKeys DbSet.</summary>
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    /// <summary>Gets or sets the TransactionFees DbSet.</summary>
    public DbSet<TransactionFee> TransactionFees => Set<TransactionFee>();

    /// <summary>Gets or sets the AuditLogs DbSet.</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>Gets or sets the WebhookPayloads DbSet.</summary>
    public DbSet<WebhookPayload> WebhookPayloads => Set<WebhookPayload>();

    /// <summary>Configures the database model mappings.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>Overrides SaveChangesAsync to automatically set CreatedAt timestamps.</summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var createdAtProp = entry.Entity.GetType().GetProperty("CreatedAt");
                if (createdAtProp != null && createdAtProp.PropertyType == typeof(DateTime))
                {
                    createdAtProp.SetValue(entry.Entity, DateTime.UtcNow);
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
