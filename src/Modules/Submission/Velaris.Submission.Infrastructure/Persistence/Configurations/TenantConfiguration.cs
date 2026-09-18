using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(t => t.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(t => t.MaxMessageSizeBytes)
            .IsRequired();

        builder.Property(t => t.MaxRecipientsPerSubmission)
            .IsRequired();

        builder.Property(t => t.MaxSmtpSessionDurationSeconds)
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .IsRequired();

        builder.Property(t => t.UpdatedAtUtc)
            .IsRequired();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_tenants_max_message_size", "\"MaxMessageSizeBytes\" > 0");
            t.HasCheckConstraint("CK_tenants_max_recipients", "\"MaxRecipientsPerSubmission\" > 0");
            t.HasCheckConstraint("CK_tenants_max_duration", "\"MaxSmtpSessionDurationSeconds\" > 0");
        });
    }
}
