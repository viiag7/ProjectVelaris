namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Relational metadata reference to attachment bytes durably stored in Object Storage (ADR-0010).
/// </summary>
public sealed class AttachmentReference
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid MessageId { get; set; }

    public string OpaqueObjectKey { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string HashAlgorithm { get; set; } = "SHA-256";

    public string HashValue { get; set; } = string.Empty;

    public string? StorageVersion { get; set; }

    public int OrderIndex { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public Message? Message { get; set; }
}
