using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class SuppressionConfiguration : IEntityTypeConfiguration<Suppression>
{
    public void Configure(EntityTypeBuilder<Suppression> builder)
    {
        builder.ToTable("suppressions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.EnvironmentId)
            .IsRequired();

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(s => s.Reason)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.Source)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(s => s.EnhancedStatusCode)
            .HasMaxLength(32);

        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(s => s.Environment)
            .WithMany(e => e.Suppressions)
            .HasForeignKey(s => s.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.EnvironmentId, s.Email })
            .IsUnique();
    }
}
