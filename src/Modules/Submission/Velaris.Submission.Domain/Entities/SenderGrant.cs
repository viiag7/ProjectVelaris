using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Authorized sender policy grant associated with a credential (ADR-0004, RF-CRE-004).
/// </summary>
public sealed class SenderGrant
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid CredentialId { get; set; }

    public SenderGrantType Type { get; set; }

    public string Value { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public Credential? Credential { get; set; }
}
