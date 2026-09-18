namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Accepted message entity preserving submission metadata, headers, body parts and deliveries.
/// </summary>
public sealed class Message
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid EnvironmentId { get; set; }

    public Guid? CredentialId { get; set; }

    public string EnvelopeSender { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public DateTimeOffset AcceptedAtUtc { get; set; }

    public string? SubmissionIp { get; set; }

    public string? ClientEhlo { get; set; }

    public string? TlsCipher { get; set; }

    public string? AuthenticatedUser { get; set; }

    public Tenant? Tenant { get; set; }

    public TenantEnvironment? Environment { get; set; }

    public Credential? Credential { get; set; }

    public ICollection<MessageHeader> Headers { get; set; } = [];

    public ICollection<MessageBodyPart> BodyParts { get; set; } = [];

    public ICollection<AttachmentReference> Attachments { get; set; } = [];

    public ICollection<Delivery> Deliveries { get; set; } = [];
}
