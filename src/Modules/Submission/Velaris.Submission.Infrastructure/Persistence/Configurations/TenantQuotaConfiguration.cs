using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class TenantQuotaConfiguration : IEntityTypeConfiguration<TenantQuota>
{
    public void Configure(EntityTypeBuilder<TenantQuota> builder)
    {
        builder.ToTable("tenant_quotas");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.TenantId)
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

        builder.HasOne(q => q.Tenant)
            .WithMany(t => t.Quotas)
            .HasForeignKey(q => q.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => new { q.TenantId, q.PeriodType, q.PeriodStartUtc })
            .IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_tenant_quotas_allocated_limit", "\"AllocatedLimit\" >= 0");
            t.HasCheckConstraint("CK_tenant_quotas_used_count", "\"UsedCount\" >= 0");
        });
    }
}
