using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence.Configurations;

public sealed class MessageHeaderConfiguration : IEntityTypeConfiguration<MessageHeader>
{
    public void Configure(EntityTypeBuilder<MessageHeader> builder)
    {
        builder.ToTable("message_headers");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.TenantId)
            .IsRequired();

        builder.Property(h => h.MessageId)
            .IsRequired();

        builder.Property(h => h.OrderIndex)
            .IsRequired();

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(h => h.Value)
            .IsRequired();

        builder.HasOne(h => h.Message)
            .WithMany(m => m.Headers)
            .HasForeignKey(h => h.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(h => new { h.MessageId, h.OrderIndex });
    }
}
