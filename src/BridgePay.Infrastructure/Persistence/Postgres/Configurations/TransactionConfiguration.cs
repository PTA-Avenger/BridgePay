namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity configuration for the Transaction entity.
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    /// <summary>Configures database schema mapping and properties for Transaction.</summary>
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.BankProvider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.BankTransactionId)
            .HasMaxLength(100);

        builder.Property(t => t.FeeAmount)
            .HasPrecision(18, 4);

        builder.Property(t => t.NetAmount)
            .HasPrecision(18, 4);

        builder.Property(t => t.PaymentMethod)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.CustomerReference)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.FailureReason)
            .HasMaxLength(250);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Indexes for performance optimization
        builder.HasIndex(t => t.MerchantId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedAt);
    }
}
