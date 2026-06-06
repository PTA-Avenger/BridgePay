namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity configuration for the TransactionFee entity.
/// </summary>
public class TransactionFeeConfiguration : IEntityTypeConfiguration<TransactionFee>
{
    /// <summary>Configures database schema mapping and properties for TransactionFee.</summary>
    public void Configure(EntityTypeBuilder<TransactionFee> builder)
    {
        builder.ToTable("TransactionFees");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.BankProvider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.TransactionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.FeePercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(f => f.FlatFee)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.HasIndex(f => new { f.BankProvider, f.TransactionType }).IsUnique();
    }
}
