namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Ordered and repeatable submitted RFC 5322 header (ADR-0010).
/// </summary>
public sealed class MessageHeader
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid MessageId { get; set; }

    public int OrderIndex { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public Message? Message { get; set; }
}
