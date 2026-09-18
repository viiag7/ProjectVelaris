using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TenantId)
            .IsRequired();

        builder.Property(m => m.EnvironmentId)
            .IsRequired();

        builder.Property(m => m.EnvelopeSender)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(m => m.Subject)
            .HasMaxLength(998);

        builder.Property(m => m.AcceptedAtUtc)
            .IsRequired();

        builder.Property(m => m.SubmissionIp)
            .HasMaxLength(45);

        builder.Property(m => m.ClientEhlo)
            .HasMaxLength(255);

        builder.Property(m => m.TlsCipher)
            .HasMaxLength(64);

        builder.Property(m => m.AuthenticatedUser)
            .HasMaxLength(128);

        builder.HasOne(m => m.Tenant)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Environment)
            .WithMany()
            .HasForeignKey(m => m.EnvironmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Credential)
            .WithMany()
            .HasForeignKey(m => m.CredentialId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => new { m.TenantId, m.AcceptedAtUtc });
    }
}
