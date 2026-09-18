using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class EnvironmentQuotaConfiguration : IEntityTypeConfiguration<EnvironmentQuota>
{
    public void Configure(EntityTypeBuilder<EnvironmentQuota> builder)
    {
        builder.ToTable("environment_quotas");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.TenantId)
            .IsRequired();

        builder.Property(q => q.EnvironmentId)
            .IsRequired();

        builder.Property(q => q.PeriodType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(q => q.PeriodStartUtc)
            .IsRequired();

        builder.Property(q => q.PeriodEndUtc)
            .IsRequired();

        builder.Property(q => q.AllocatedLimit)
            .IsRequired();

        builder.Property(q => q.UsedCount)
            .IsRequired();

        builder.HasOne(q => q.Environment)
            .WithMany(e => e.Quotas)
            .HasForeignKey(q => q.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => new { q.EnvironmentId, q.PeriodType, q.PeriodStartUtc })
            .IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_environment_quotas_allocated_limit", "\"AllocatedLimit\" >= 0");
            t.HasCheckConstraint("CK_environment_quotas_used_count", "\"UsedCount\" >= 0");
        });
    }
}
