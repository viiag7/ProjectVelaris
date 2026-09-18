namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Versioned SCRAM verifier material for a credential (ADR-0009, ADR-0015).
/// Never stores a recoverable password.
/// </summary>
public sealed class CredentialVerifier
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid CredentialId { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }

    public string Mechanism { get; set; } = "SCRAM-SHA-256";

    public byte[] Salt { get; set; } = [];

    public int IterationCount { get; set; }

    public byte[] StoredKey { get; set; } = [];

    public byte[] ServerKey { get; set; } = [];

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public Credential? Credential { get; set; }
}
