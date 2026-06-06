namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity configuration for the Refund entity.
/// </summary>
public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    /// <summary>Configures database schema mapping and properties for Refund.</summary>
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Amount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.RequestedAt)
            .IsRequired();

        // Relationship
        builder.HasOne(r => r.Transaction)
            .WithMany()
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.TransactionId);
    }
}
