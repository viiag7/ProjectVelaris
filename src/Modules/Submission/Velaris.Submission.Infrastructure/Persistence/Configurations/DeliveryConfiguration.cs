using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("deliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.EnvironmentId)
            .IsRequired();

        builder.Property(d => d.MessageId)
            .IsRequired();

        builder.Property(d => d.RecipientEmail)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(d => d.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(d => d.Message)
            .WithMany(m => m.Deliveries)
            .HasForeignKey(d => d.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Tenant)
            .WithMany()
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Environment)
            .WithMany()
            .HasForeignKey(d => d.EnvironmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constrain Message-to-Delivery cardinality and recipient uniqueness per message
        builder.HasIndex(d => new { d.MessageId, d.RecipientEmail })
            .IsUnique();
    }
}
