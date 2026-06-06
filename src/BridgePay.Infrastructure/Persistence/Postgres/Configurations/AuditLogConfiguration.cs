namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// EF Core configuration mapping for the AuditLog entity.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <summary>Configures table mappings, key constraints, lengths, and indexes.</summary>
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Details)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // Max length for IPv6/IPv4-mapped addresses

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.HasIndex(x => x.MerchantId);
        builder.HasIndex(x => x.Timestamp);
    }
}
