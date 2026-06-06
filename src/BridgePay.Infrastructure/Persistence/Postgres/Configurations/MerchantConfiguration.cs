namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity configuration for the Merchant entity.
/// </summary>
public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    /// <summary>Configures database schema mapping and properties for Merchant.</summary>
    public void Configure(EntityTypeBuilder<Merchant> builder)
    {
        builder.ToTable("Merchants");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.BusinessName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.ApiKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.ApiSecret)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        // Indexes for fast lookup
        builder.HasIndex(m => m.AuthUserId).IsUnique();
        builder.HasIndex(m => m.ApiKey).IsUnique();

        // Relationships
        builder.HasMany(m => m.Transactions)
            .WithOne()
            .HasForeignKey(t => t.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.ApiKeys)
            .WithOne(k => k.Merchant)
            .HasForeignKey(k => k.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
