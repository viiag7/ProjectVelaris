using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class AttachmentReferenceConfiguration : IEntityTypeConfiguration<AttachmentReference>
{
    public void Configure(EntityTypeBuilder<AttachmentReference> builder)
    {
        builder.ToTable("attachment_references");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.MessageId)
            .IsRequired();

        builder.Property(a => a.OpaqueObjectKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(a => a.FileName)
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.SizeBytes)
            .IsRequired();

        builder.Property(a => a.HashAlgorithm)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(a => a.HashValue)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(a => a.StorageVersion)
            .HasMaxLength(128);

        builder.Property(a => a.OrderIndex)
            .IsRequired();

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(a => a.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => new { a.MessageId, a.OrderIndex });
    }
}
