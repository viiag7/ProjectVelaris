using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class CredentialVerifierConfiguration : IEntityTypeConfiguration<CredentialVerifier>
{
    public void Configure(EntityTypeBuilder<CredentialVerifier> builder)
    {
        builder.ToTable("credential_verifiers");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.TenantId)
            .IsRequired();

        builder.Property(v => v.CredentialId)
            .IsRequired();

        builder.Property(v => v.Version)
            .IsRequired();

        builder.Property(v => v.IsActive)
            .IsRequired();

        builder.Property(v => v.Mechanism)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(v => v.Salt)
            .IsRequired();

        builder.Property(v => v.IterationCount)
            .IsRequired();

        builder.Property(v => v.StoredKey)
            .IsRequired();

        builder.Property(v => v.ServerKey)
            .IsRequired();

        builder.Property(v => v.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(v => v.Credential)
            .WithMany(c => c.Verifiers)
            .HasForeignKey(v => v.CredentialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.CredentialId, v.Version })
            .IsUnique();
    }
}
