namespace BridgePay.Infrastructure.Persistence.Postgres.Configurations;

using BridgePay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// EF Core configuration mapping for the WebhookPayload entity.
/// </summary>
public class WebhookPayloadConfiguration : IEntityTypeConfiguration<WebhookPayload>
{
    /// <summary>Configures table mappings, key constraints, lengths, and indexes.</summary>
    public void Configure(EntityTypeBuilder<WebhookPayload> builder)
    {
        builder.ToTable("WebhookPayloads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BankProvider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Payload)
            .IsRequired(); // Store full raw JSON content

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.HasIndex(x => x.ReceivedAt);
    }
}
