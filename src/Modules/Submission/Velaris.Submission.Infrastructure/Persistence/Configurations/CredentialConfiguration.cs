using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("credentials");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.EnvironmentId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(c => c.State)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(c => c.Tenant)
            .WithMany(t => t.Credentials)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Environment)
            .WithMany(e => e.Credentials)
            .HasForeignKey(c => c.EnvironmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.EnvironmentId, c.Name })
            .IsUnique();
    }
}
