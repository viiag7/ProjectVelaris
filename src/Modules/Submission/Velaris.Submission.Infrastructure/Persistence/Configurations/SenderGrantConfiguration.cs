using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class SenderGrantConfiguration : IEntityTypeConfiguration<SenderGrant>
{
    public void Configure(EntityTypeBuilder<SenderGrant> builder)
    {
        builder.ToTable("sender_grants");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.TenantId)
            .IsRequired();

        builder.Property(g => g.CredentialId)
            .IsRequired();

        builder.Property(g => g.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(g => g.Value)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(g => g.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(g => g.Credential)
            .WithMany(c => c.SenderGrants)
            .HasForeignKey(g => g.CredentialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => new { g.CredentialId, g.Type, g.Value })
            .IsUnique();
    }
}
