namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity configuration for the ApiKey entity.
/// </summary>
public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    /// <summary>Configures database schema mapping and properties for ApiKey.</summary>
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(k => k.Secret)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(k => k.IsActive)
            .IsRequired();

        builder.Property(k => k.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(k => k.Key).IsUnique();
        builder.HasIndex(k => k.MerchantId);

        // Relationship
        builder.HasOne(k => k.Merchant)
            .WithMany(m => m.ApiKeys)
            .HasForeignKey(k => k.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
