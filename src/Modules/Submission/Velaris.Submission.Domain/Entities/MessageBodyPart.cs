namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Parsed MIME body part preserving encoding metadata and content (ADR-0010).
/// </summary>
public sealed class MessageBodyPart
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid MessageId { get; set; }

    public int OrderIndex { get; set; }

    public string MediaType { get; set; } = string.Empty;

    public string? Charset { get; set; }

    public string? ContentTransferEncoding { get; set; }

    public string? ContentId { get; set; }

    public string? ContentDisposition { get; set; }

    public string Content { get; set; } = string.Empty;

    public Message? Message { get; set; }
}
