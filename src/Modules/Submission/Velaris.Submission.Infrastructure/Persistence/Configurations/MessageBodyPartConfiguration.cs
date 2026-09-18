using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class MessageBodyPartConfiguration : IEntityTypeConfiguration<MessageBodyPart>
{
    public void Configure(EntityTypeBuilder<MessageBodyPart> builder)
    {
        builder.ToTable("message_body_parts");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TenantId)
            .IsRequired();

        builder.Property(b => b.MessageId)
            .IsRequired();

        builder.Property(b => b.OrderIndex)
            .IsRequired();

        builder.Property(b => b.MediaType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(b => b.Charset)
            .HasMaxLength(64);

        builder.Property(b => b.ContentTransferEncoding)
            .HasMaxLength(64);

        builder.Property(b => b.ContentId)
            .HasMaxLength(256);

        builder.Property(b => b.ContentDisposition)
            .HasMaxLength(64);

        builder.Property(b => b.Content)
            .IsRequired();

        builder.HasOne(b => b.Message)
            .WithMany(m => m.BodyParts)
            .HasForeignKey(b => b.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.MessageId, b.OrderIndex });
    }
}
